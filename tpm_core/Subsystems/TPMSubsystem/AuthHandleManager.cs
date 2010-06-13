// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common;
using System.Collections.Generic;
using Iaik.Utils.SwapUtils;
using Iaik.Tc.TPM.Library.Common.Handles;
using log4net;
using System.Text;
using System.Threading;
using Iaik.Utils.Locking;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{

	/// <summary>
	/// Manages the currently active authorization handles of a single tpm device
	/// </summary>
	public class AuthHandleManager : SwapManager<AuthHandleItem>, IAuthHandleManager
	{

		/// <summary>
		/// The TPMContext to manage
		/// </summary>
		private TPMContext _tpmContext;
		
		/// <summary>
		/// Contains all auth handle items
		/// </summary>
		private AuthHandleCollection _authHandles = new AuthHandleCollection();
		
		/// <summary>
		/// Logger
		/// </summary>
		protected ILog _logger = LogManager.GetLogger("AuthHandleManager");
		
		public AuthHandleManager (TPMContext tpmContext)
			:base(new Aging())
		{
			_tpmContext = tpmContext;
		}
		

		private uint LoadedSessions
		{
			get
			{
				return (uint)_authHandles.FindAuthHandles(AuthHandleItem.AuthHandleStatus.SwappedIn).Count;
			}
		}
		
		
		private uint AvailableSessionSlots
		{
			get
			{
				Parameters parameters = new Parameters ();
				parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY);
				parameters.AddPrimitiveType ("subCap", CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_AUTHSESS);
			
				TPMCommandRequest request = new TPMCommandRequest(TPMCommandNames.TPM_CMD_GetCapability, parameters);
				TPMCommandResponse response = _tpmContext.TPM.Process(request);
				
				if(response.Status == false)
					throw new TPMRequestException("Unknown error");
				
				return response.Parameters.GetValueOf<uint>(CapabilityData.PARAM_PROP_MAX_AUTHSESS);
			}
		}
		
		#region IAuthHandleManager implementation


		/// <summary>
		/// Acquires an exclusive AuthHandle lock
		/// </summary>
		/// <returns></returns>
		public LockContext AcquireLock()
		{
			return new LockContext(_authHandles, "AuthHandleManager");
		}

        /// <summary>
        /// Ensures that a free slot is available.
        /// Be sure that this method is only called with a locked auth handle manager.
        /// </summary>
        public void EnsureFreeSlot()
        {
            //If no free session slots are available, swap out a single session slot
            while (AvailableSessionSlots <= LoadedSessions)
                SwapOut();
        }
		
		/// <summary>
		/// Reserves the number of session slots the given command requires on the tpm.
		/// </summary>
		/// <remarks>
		/// If the command supports OIAP auth handles, it is also possible for the AuthHandleManager to recycle
		/// existing OIAP AuthHandles.
		/// 
		/// If the command needs OSAP auth handles the AuthHandleManager always creates new sessions, because OSAP is
		/// used for auth data insertion, so nothing happens in here for OSAP
		/// </remarks>
		/// <param name="cmd"></param>
		/// <param name="tpmSession"></param>
		public void ReserveAuthHandleSlots(IAuthorizableCommand cmd)
		{
			
			using(new LockContext(_authHandles, string.Format("AuthHandleManager::ReserveAuthHandleSlots {0}", cmd)))
			{
				foreach(AuthSessionNum authSession in new AuthSessionNum[]{AuthSessionNum.Auth1, AuthSessionNum.Auth2})
				{
					
					HMACKeyInfo keyInfo = cmd.GetKeyInfo(authSession);
					
					if(keyInfo == null)
						continue;
					
					bool useOIAP = false;
					if(cmd.SupportsAuthType(AuthHandle.AuthType.OIAP))
						useOIAP = true;
						
					
					if(FindReservedHandle(cmd, authSession) != null)
						continue;						
					
					//We only cache OIAP sessions, so maybe there is a free
					//OIAP session we can use
					if(useOIAP)
					{
						AuthHandleItem handle = FindFreeAuthHandle(AuthHandleItem.AuthHandleStatus.SwappedIn);

																	
						if(handle == null)
						{
							handle = FindFreeAuthHandle(AuthHandleItem.AuthHandleStatus.SwappedOut);
						}
						

						if(handle != null)
						{
							handle.AssociatedCommand = new KeyValuePair<AuthSessionNum, IAuthorizableCommand>(authSession, cmd);							
						}
					}
				}
			}
			
		}
		
		public AuthHandle GetAuthHandle (IAuthorizableCommand cmd, AuthSessionNum authSession)
		{
			AuthHandleItem reservedHandle = null;
			using(new LockContext(_authHandles, string.Format("AuthHandleManager::GetAuthHandle {0}", cmd)))
			{
				reservedHandle = FindReservedHandle(cmd, authSession);
				
				
				// No reserved auth handle was found, create a new one
				if(reservedHandle == null)
				{
					reservedHandle = CreateAuthHandle(cmd, authSession);
				}
			}
			
			return reservedHandle.AuthHandle;
		}
		
		
		private AuthHandleItem CreateAuthHandle(IAuthorizableCommand cmd, AuthSessionNum authSession)
		{

			if(cmd.SupportsAuthType(AuthHandle.AuthType.OIAP))
			{


				TPMCommandRequest oiapRequest = new TPMCommandRequest(TPMCommandNames.TPM_CMD_OIAP, new Parameters());
				TPMCommandResponse oiapResponse = _tpmContext.TPM.Process(oiapRequest, cmd.CommandAuthHelper, cmd.KeyManager);
				if(oiapResponse.Status == false)
					throw new TPMRequestException("Unknown error while creating oiap auth handle");
				
				AuthHandle newAuthHandle = oiapResponse.Parameters.GetValueOf<AuthHandle>("auth_handle");
				AuthHandleItem authHandleItem = new AuthHandleItem(newAuthHandle, AuthHandleItem.AuthHandleStatus.SwappedIn);
				authHandleItem.AssociatedCommand = new KeyValuePair<AuthSessionNum, IAuthorizableCommand>(authSession, cmd);
				
				using(new LockContext(_authHandles, "CreateAuthHandle OIAP"))
				{
					_authHandles.AddAuthHandle(authHandleItem);
					AddNewItem(authHandleItem);
				}
				
				return authHandleItem;
			}
			else if(cmd.SupportsAuthType(AuthHandle.AuthType.OSAP))
			{
				Parameters parameters = new Parameters();
				parameters.AddPrimitiveType("entity_msb", TPMEntityTypeMSB.TPM_ET_XOR);
				parameters.AddPrimitiveType("entity_lsb", cmd.GetEntityType(authSession));
				string identifier = cmd.GetHandle(authSession);
				
				if(identifier == null)
					throw new TPMRequestException("Missing entity value for OSAP session");
				
				parameters.AddPrimitiveType("entity_value", identifier);
				
				TPMCommandRequest osapRequest = new TPMCommandRequest(TPMCommandNames.TPM_CMD_OSAP, parameters);
				
				TPMCommandResponse osapResponse = _tpmContext.TPM.Process(osapRequest, cmd.CommandAuthHelper, cmd.KeyManager);
				if(osapResponse.Status == false)
					throw new TPMRequestException("Unknown error while creating osap auth handle");
					
				AuthHandle newAuthHandle = osapResponse.Parameters.GetValueOf<AuthHandle>("auth_handle");
				AuthHandleItem authHandleItem = new AuthHandleItem(newAuthHandle, AuthHandleItem.AuthHandleStatus.SwappedIn);
				authHandleItem.AssociatedCommand = new KeyValuePair<AuthSessionNum, IAuthorizableCommand>(authSession, cmd);
				
				using(new LockContext(_authHandles, "CreateAuthHandle OSAP"))
				{
					_authHandles.AddAuthHandle(authHandleItem);
					AddNewItem(authHandleItem);
				}
				
				return authHandleItem;
			}
			else 
				throw new NotSupportedException("Command does not support a suitable AuthType");
		}
		
		public void ReleaseAuthHandles (IAuthorizableCommand cmd)
		{
			using(new LockContext(_authHandles, "AuthHandleManager::ReleaseAuthHandles"))
			{
				foreach(AuthHandleItem handle in FindReservedHandles(cmd))
				{
					handle.AssociatedCommand = null;
				}
			}
		}
		
		public void MarkAsUsed(IEnumerable<AuthHandle> authHandles)
		{
			using(new LockContext(_authHandles, "AuthHandleManager::LoadAuthHandle"))
			{
				List<UInt64> ids = new List<UInt64>();
				foreach(AuthHandle authHandle in authHandles)
					ids.Add(ItemToId(FindAuthHandleItem(authHandle)).Value);
				_replacementAlgorithm.RegisterUsed(ids);
				_replacementAlgorithm.Update();
				
			}
		}
		
		public void LoadAuthHandle(AuthHandle authHandle)
		{
			using(new LockContext(_authHandles, "AuthHandleManager::LoadAuthHandle"))
			{
				List<AuthHandleItem> myAuthHandles =_authHandles.FindAuthHandles(AuthHandleItem.AuthHandleStatus.SwappedOut);
				_logger.DebugFormat("LoadAuthHandle: #{0}", myAuthHandles.Count);
				
				foreach(AuthHandleItem handleItem in myAuthHandles)
				{
					_logger.DebugFormat("LoadAuthHandles: Found auth handle {0} with status {1}", handleItem.AuthHandle, handleItem.Status);
					if(handleItem.AuthHandle == authHandle &&
					   handleItem.Status != AuthHandleItem.AuthHandleStatus.SwappedIn)
					{

						SwapIn(handleItem);
						_logger.DebugFormat("LoadAuthHandles: Swapped in {0}", handleItem.AuthHandle);
					}
				}
			}
		}
		
		public void DestroyAuthHandles (IAuthorizableCommand cmd)
		{
			try
			{
				using(new LockContext(_authHandles, "AuthHandleManager::DestroyAuthHandles"))
				{
					foreach(AuthHandleItem item in FindReservedHandles(cmd))
					{
						InternalDestroyHandle(item);
					}
				}
			}
			catch(Exception ex)
			{
				_logger.FatalFormat("ERROR: destroyAuthHandles {0}", ex);
				throw;
			}
		}
		
		public void DestroyAuthHandles (IAuthorizableCommand cmd, params AuthHandle[] authHandles)
		{
		
			using(new LockContext(_authHandles, "AuthHandleManager::DestroyAuthHandles2"))
			{
				foreach(AuthHandle authHandle in authHandles)
				{
					AuthHandleItem item = FindAuthHandleItem(authHandle);
					
					if(item == null)
						continue;
					
					if(item.AssociatedCommand != null && item.AssociatedCommand.Value.Value == cmd)
					{
						InternalDestroyHandle(item);
					}
				}			
			}
		}
		
		/// <summary>
		/// Really destroys the auth handle on the tpm AND in the local cache
		/// </summary>
		/// <param name="item">
		/// A <see cref="AuthHandleItem"/>
		/// </param>
		private void InternalDestroyHandle(AuthHandleItem item)
		{
			Parameters parameters = new Parameters();
			parameters.AddValue("handle", item.AuthHandle);
			parameters.AddPrimitiveType("ignore_tpm_error", true);
			TPMCommandRequest destroyHandleRequest = new TPMCommandRequest(TPMCommandNames.TPM_CMD_FlushSpecific, parameters);
			
			TPMCommandResponse destroyHandleResponse = _tpmContext.TPM.Process(destroyHandleRequest);
			
			if(!destroyHandleResponse.Status)
				throw new TPMRequestException("Error on flushing auth handle context, aborting");
			
			_logger.DebugFormat("Authhandles before remove: {0}", _authHandles);
			InternalRemoveAuthHandle(item);
			_logger.DebugFormat("Authhandles after remove: {0}", _authHandles);
			
		}
		
		
		public void RemoveAuthHandles (IAuthorizableCommand cmd)
		{
			using(new LockContext(_authHandles, "AuthHandleManager::RemoveAuthHandles"))
			{
				foreach(AuthHandleItem handle in FindReservedHandles(cmd))
				{
					_authHandles.RemoveAuthHandle(handle);
				}
			}
		}

        public void RemoveAuthHandles(IAuthorizableCommand cmd, AuthHandle authHandle)
        {

            using (new LockContext(_authHandles, "AuthHandleManager::RemoveAuthHandles2"))
            {
                AuthHandleItem item = FindAuthHandleItem(authHandle);

                if (item == null)
                    return;

                if (item.AssociatedCommand != null && item.AssociatedCommand.Value.Value == cmd)
                {
                    InternalRemoveAuthHandle(item);
                }
                else
                    return;

            }
        }

		
		private void RemoveAuthHandle(AuthHandleItem item)
		{
			using(new LockContext(_authHandles, "AuthHandleManager::RemoveAuthHandles2"))
			{
				InternalRemoveAuthHandle(item);
			}
		}

		private void InternalRemoveAuthHandle(AuthHandleItem item)
		{
            RemoveItem(item);
			_authHandles.RemoveAuthHandle(item);
            
		}
		
		#endregion
		
		#region SwapManager overrides

		/// <summary>
		/// Performs the actual swap-in operation
		/// </summary>
		/// <param name="item"></param>
		protected override void SwappedIn (AuthHandleItem item)
		{
			uint oldHandle = item.AuthHandle.Handle;
			if(item.Status != AuthHandleItem.AuthHandleStatus.SwappedOut)
				throw new ArgumentException("Invalid auth handle state for swap in operation");
			
			using(new LockContext(_authHandles, "AuthHandleManager::SwappedIn"))
			{
				if(AvailableSessionSlots <= LoadedSessions)
					SwapOut();
				
				
				Parameters swapInParameters = new Parameters();
				swapInParameters.AddValue("handle", item.AuthHandle);
				swapInParameters.AddPrimitiveType("context_blob", item.AuthHandle.ContextBlob);
				
				TPMCommandRequest swapInRequest = new TPMCommandRequest(TPMCommandNames.TPM_CMD_LoadContext, swapInParameters);
				
				TPMCommandResponse swapInResponse =_tpmContext.TPM.Process(swapInRequest);
				if(swapInResponse.Status == false)
					throw new TPMRequestException("Unknown error while swap in operation");
				
				item.AuthHandle.Handle = swapInResponse.Parameters.GetValueOf<ITPMHandle>("handle").Handle;
				item.Status = AuthHandleItem.AuthHandleStatus.SwappedIn;
			}
		}

		/// <summary>
		/// Performs the actual swap-out operation
		/// </summary>
		/// <param name="item"></param>
		protected override void SwappedOut (AuthHandleItem item)
		{
			if(item.Status != AuthHandleItem.AuthHandleStatus.SwappedIn)
				throw new ArgumentException("Invalid auth handle state for swap out operation");
			
			using(new LockContext(_authHandles, "AuthHandleManager::SwappedOut"))
			{
				Parameters swapOutParameters = new Parameters();
				swapOutParameters.AddValue("handle", item.AuthHandle);
				TPMCommandRequest swapOutRequest = new TPMCommandRequest(TPMCommandNames.TPM_CMD_SaveContext, swapOutParameters);
				
				TPMCommandResponse swapOutResponse =_tpmContext.TPM.Process(swapOutRequest);
	
				if(swapOutResponse.Status == false)
					throw new TPMRequestException("Unknown error while swap out operation");
				
				
				item.Status = AuthHandleItem.AuthHandleStatus.SwappedOut;
				item.AuthHandle.ContextBlob = swapOutResponse.Parameters.GetValueOf<byte[]>("context_blob");
			}
		}

		#endregion

		private AuthHandleItem FindFreeAuthHandle(AuthHandleItem.AuthHandleStatus status)
		{
						
			foreach(AuthHandleItem handle in _authHandles.FindAuthHandles(status))
			{
				if(handle.AssociatedCommand == null)
					return handle;
			}
			
			return null;
		}
		
		private AuthHandleItem FindReservedHandle(IAuthorizableCommand cmd, AuthSessionNum authSession)
		{
			foreach(AuthHandleItem handle in _authHandles.FindAuthHandles(AuthHandleItem.AuthHandleStatus.SwappedIn))
			{
				if(handle.AssociatedCommand == null)
					continue;
					
				if(handle.AssociatedCommand.Value.Key == authSession &&
				   handle.AssociatedCommand.Value.Value == cmd)
					return handle;
			}
			
			foreach(AuthHandleItem handle in _authHandles.FindAuthHandles(AuthHandleItem.AuthHandleStatus.SwappedOut))
			{
				if(handle.AssociatedCommand == null)
					continue;
				
				if(handle.AssociatedCommand.Value.Key == authSession &&
				   handle.AssociatedCommand.Value.Value == cmd)
					return handle;
			}
			
			
			return null;
		}
		
		private AuthHandleItem[] FindReservedHandles(IAuthorizableCommand cmd)
		{
			List<AuthHandleItem> returnValues = new List<AuthHandleItem>();
			
			foreach(AuthHandleItem handle in _authHandles.FindAuthHandles(AuthHandleItem.AuthHandleStatus.SwappedIn))
			{
				if(handle.AssociatedCommand == null)
					continue;
					
				if(handle.AssociatedCommand.Value.Value == cmd)
				{
					returnValues.Add(handle);
				}
			}
			
			foreach(AuthHandleItem handle in _authHandles.FindAuthHandles(AuthHandleItem.AuthHandleStatus.SwappedOut))
			{
				if(handle.AssociatedCommand == null)
					continue;
				
				if(handle.AssociatedCommand.Value.Value == cmd)
				{
					returnValues.Add(handle);
				}
			}
			
			return returnValues.ToArray();
		}
		
		private AuthHandleItem FindAuthHandleItem(AuthHandle authHandle)
		{
			foreach(AuthHandleItem item in _authHandles)
			{
				if(item.AuthHandle == authHandle)
					return item;
			}
			
			return null;
		}

	}
	
	public class AuthHandleCollection : IEnumerable<AuthHandleItem>
	{
		/// <summary>
		/// Contains all current authorization handles
		/// @key tpmSessionIdentifier
		/// </summary>
		List<AuthHandleItem> _authHandles = new List<AuthHandleItem>();
	
		public int Count
		{
			get{ return _authHandles.Count; }
		}
		
		/// <summary>
		/// Adds the specified auth handle item to the list
		/// </summary>
		/// <param name="authHandleItem"></param>
		public void AddAuthHandle(AuthHandleItem authHandleItem)
		{
			//Check if there is an old obsolete authhandle with the same id
			foreach(AuthHandleItem item in _authHandles)
			{
				if(item.AuthHandle.Handle == authHandleItem.AuthHandle.Handle)
				{
					_authHandles.Remove(item);
					break;
				}
			}
			
			_authHandles.Add(authHandleItem);
		}
		
		/// <summary>
		/// Removes the specified authhandle
		/// </summary>
		/// <param name="tpmSessionIdentifier"></param>
		/// <param name="authHandleItem"></param>
		public void RemoveAuthHandle(AuthHandleItem authHandleItem)
		{
			_authHandles.Remove(authHandleItem);
		}
		
		/// <summary>
		/// Finds the Auth handles with the specified status
		/// </summary>
		/// <remarks>
		/// The returned list is sorted by last used time
		/// </remarks>
		/// <param name="status"></param>
		/// <returns></returns>
		public List<AuthHandleItem> FindAuthHandles(AuthHandleItem.AuthHandleStatus status)
		{
			List<AuthHandleItem> items = new List<AuthHandleItem>();
			foreach(AuthHandleItem handle in _authHandles)
			{
				if(handle.Status == status)
					items.Add(handle);
			}
				
			items.Sort(delegate(AuthHandleItem left, AuthHandleItem right)
				{
					return left.LastUsed.CompareTo(right.LastUsed);
				}
			);
			
			return items;
			
		}
		
		#region IEnumerable<AuthHandleItem> implementation
		public IEnumerator<AuthHandleItem> GetEnumerator ()
		{
			return _authHandles.GetEnumerator();
		}
		
		#endregion
		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return _authHandles.GetEnumerator();
		}
		
		#endregion
		
		public override string ToString ()
		{
			StringBuilder str = new StringBuilder();
			str.AppendFormat("[AuthHandleCollection: Count={0}\n", Count);
			
			for(int i = 0; i<_authHandles.Count; i++)
			{
				str.AppendFormat("#{0}: {1} 0x{2:X}\n", i, _authHandles[i].Status, _authHandles[i].AuthHandle.Handle);
			}
			str.Append("\n");
			
			return str.ToString();
		}

	}
	
	public class AuthHandleItem
	{
		public enum AuthHandleStatus
		{
			/// <summary>
			/// Authorization handle is loaded and can be used
			/// </summary>
			SwappedIn,
			
			/// <summary>
			/// Authorization handle is swapped out, swap in to use it
			/// </summary>
			SwappedOut,
			
			/// <summary>
			/// Authorization handle needs to be created
			/// </summary>
			NotLoaded
		}
		
		private int _lastUsed = Environment.TickCount;
		
		/// <summary>
		/// Returns the ticks the handle was last used
		/// </summary>
		public int LastUsed
		{
			get{  return _lastUsed; }
		}
		
		private AuthHandle _authHandle = null;
		
		/// <summary>
		/// Returns the TPM AuthHandle
		/// </summary>
		public AuthHandle AuthHandle
		{
			get{ return _authHandle; }
		}
		
		private AuthHandleStatus _status = AuthHandleStatus.NotLoaded;
		
		/// <summary>
		/// Returns the Swap/Load state of this Authorization handle
		/// </summary>
		public AuthHandleStatus Status
		{
			get{ return _status; }
			set{ _status = value;}
		}
		
		
		private KeyValuePair<AuthSessionNum, IAuthorizableCommand>? _associatedCommand = null;
		
		/// <summary>
		/// Gets or Sets the command which uses this AuthHandle
		/// </summary>
		public KeyValuePair<AuthSessionNum, IAuthorizableCommand>? AssociatedCommand
		{
			get{ return _associatedCommand; }
			set{ _associatedCommand = value;}
		}
		
		public AuthHandleItem(AuthHandle authHandle, AuthHandleStatus status)
		{
			_authHandle = authHandle;
			_status = status;
		}
		
		/// <summary>
		/// Updates the last used time
		/// </summary>
		public void UpdateLastUsed()
		{
			_lastUsed = Environment.TickCount;
		}
	}
}
