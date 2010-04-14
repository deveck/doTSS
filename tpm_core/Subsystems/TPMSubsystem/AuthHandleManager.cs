// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{

	/// <summary>
	/// Manages the currently active authorization handles of a single tpm device
	/// </summary>
	public class AuthHandleManager : IAuthHandleManager
	{

		/// <summary>
		/// The TPMContext to manage
		/// </summary>
		private TPMContext _tpmContext;
		
		/// <summary>
		/// Contains all loaded and swapped AuthHandles
		/// </summary>
		private Dictionary<int, AuthHandleCollection> _authHandles = new Dictionary<int, AuthHandleCollection>();
		
		/// <summary>
		/// Slots that have been reserved by tpm commands
		/// </summary>
		private int _reservedSlots = 0;
		
		public AuthHandleManager (TPMContext tpmContext)
		{
			_tpmContext = tpmContext;
		}
		

		
		private uint AvailableSessionSlots
		{
			get
			{
				Parameters parameters = new Parameters ();
				parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY);
				parameters.AddPrimitiveType ("subCap", CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_SESSIONS);
			
				TPMCommandRequest request = new TPMCommandRequest(TPMCommandNames.TPM_CMD_GetCapability, parameters);
				TPMCommandResponse response = _tpmContext.TPM.Process(request);
				
				if(response.Status == false)
					throw new TPMRequestException("Unknown error");
				
				return response.Parameters.GetValueOf<uint>(CapabilityData.PARAM_PROP_MAX_SESSIONS);
			}
		}
		
		#region IAuthHandleManager implementation
		/// <summary>
		/// Reserves the number of session slots this command requires on the tpm.
		/// </summary>
		/// <remarks>
		/// If the command supports OIAP auth handles, it is also possible for the AuthHandleManager to recycle
		/// existing OIAP AuthHandles (it only uses handles associated with the same tpmSession).
		/// 
		/// If the command needs OSAP auth handles the AuthHandleManager always creates new sessions (for the present)
		/// </remarks>
		/// <param name="cmd"></param>
		/// <param name="tpmSession"></param>
		public void ReserveAuthHandleSlots(IAuthorizableCommand cmd, object tpmSession)
		{
			if(!(tpmSession is int))
				throw new NotSupportedException("tpmSession need to be of type int");
			
			int authHandlesCount = 0;
			
			foreach(AuthSessionNum authSession in new AuthSessionNum[]{AuthSessionNum.Auth1, AuthSessionNum.Auth2})
			{
				HMACKeyInfo keyInfo = cmd.GetKeyInfo(authSession);
				
				if(keyInfo == null)
					continue;
			}
			
			
			
		}
		
		public AuthHandle GetAuthHandle (IAuthorizableCommand cmd, object tpmSession)
		{
			if(!(tpmSession is int))
				throw new NotSupportedException("tpmSession need to be of type int");
			
			return null;
		}
		
		
		public void ReleaseAuthHandle (AuthHandle authHandle)
		{
			throw new System.NotImplementedException();
		}
		
		#endregion

	}
	
	public class AuthHandleCollection
	{
		 
		
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
		}
		
		
		//public AuthHandleItem(AuthHandle authHandle, 
		
		/// <summary>
		/// Updates the last used time
		/// </summary>
		public void UpdateLastUsed()
		{
			_lastUsed = Environment.TickCount;
		}
	}
}
