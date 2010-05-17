// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using Iaik.Utils.SwapUtils;
using Iaik.Utils.Locking;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Tc.TPM.Keystore;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles;
using System.Threading;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;


namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{

	/// <summary>
	/// Manages the currently active authorization handles of a single tpm device
	/// </summary>
	public class KeyManager : SwapManager<KeyHandleItem>, IKeyManager
	{
		/// <summary>
		/// Contains all auth handle items
		/// </summary>
		private KeyHandleCollection _keyHandles = new KeyHandleCollection();
		
		/// <summary>
		/// Lock target for AcquireLock
		/// </summary>
		private object _lockTarget = new object();
		
		/// <summary>
		/// The TPM Context this key manager belongs to
		/// </summary>
		private TPMContext _tpmContext;
	
		public KeyManager (TPMContext tpmContext)
			:base(new Aging())
		{
			_tpmContext = tpmContext;
		}
		
		

		private uint LoadedKeys
		{
			get
			{
				return (uint)_keyHandles.FindKeyHandles(KeyHandleItem.KeyHandleStatus.SwappedIn).Count;
			}
		}
		
		
		private uint AvailableKeySlots
		{
			get
			{
				Parameters parameters = new Parameters ();
				parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY);
				parameters.AddPrimitiveType ("subCap", CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_KEYS);
			
				TPMCommandRequest request = new TPMCommandRequest(TPMCommandNames.TPM_CMD_GetCapability, parameters);
				TPMCommandResponse response = _tpmContext.TPM.Process(request);
				
				if(response.Status == false)
					throw new TPMRequestException("Unknown error");
				
				return response.Parameters.GetValueOf<uint>(CapabilityData.PARAM_PROP_MAX_KEYS);
			}
		}
		
		
		
		#region SwapManager overrides

		/// <summary>
		/// Performs the actual swap-in operation
		/// </summary>
		/// <param name="item"></param>
		protected override void SwappedIn (KeyHandleItem item)
		{
			if(item.Status != KeyHandleItem.KeyHandleStatus.SwappedOut)
				throw new ArgumentException("Invalid key handle state for swap in operation");
			
			if(AvailableKeySlots <= LoadedKeys)
				SwapOut();
			
			
			Parameters swapInParameters = new Parameters();
			swapInParameters.AddValue("handle", item.KeyHandle);
			
			TPMCommandRequest swapInRequest = new TPMCommandRequest(TPMCommandNames.TPM_CMD_LoadContext, swapInParameters);
			
			TPMCommandResponse swapInResponse =_tpmContext.TPM.Process(swapInRequest);
			if(swapInResponse.Status == false)
				throw new TPMRequestException("Unknown error while swap in operation");
			
			item.KeyHandle.Handle = swapInResponse.Parameters.GetValueOf<ITPMHandle>("handle").Handle;
		}

		/// <summary>
		/// Performs the actual swap-out operation
		/// </summary>
		/// <param name="item"></param>
		protected override void SwappedOut (KeyHandleItem item)
		{
			if(item.Status != KeyHandleItem.KeyHandleStatus.SwappedIn)
				throw new ArgumentException("Invalid auth handle state for swap out operation");
			
			Parameters swapOutParameters = new Parameters();
			swapOutParameters.AddValue("handle", item.KeyHandle);
			TPMCommandRequest swapOutRequest = new TPMCommandRequest(TPMCommandNames.TPM_CMD_SaveContext, swapOutParameters);
			
			TPMCommandResponse swapOutResponse =_tpmContext.TPM.Process(swapOutRequest);

			if(swapOutResponse.Status == false)
				throw new TPMRequestException("Unknown error while swap out operation");
			
			//For key handles the tpm does not release the memory after save context
			TPMCommandRequest flushRequest = new TPMCommandRequest(TPMCommandNames.TPM_CMD_FlushSpecific, swapOutParameters);
			TPMCommandResponse flushResponse = _tpmContext.TPM.Process(flushRequest);
			
			if(flushResponse.Status == false)
				throw new TPMRequestException("Unknown error while swap out (flush) operation");
			
			
			
			item.Status = KeyHandleItem.KeyHandleStatus.SwappedOut;
			item.KeyHandle.ContextBlob = swapOutResponse.Parameters.GetValueOf<byte[]>("context_blob");
		}


		#endregion



		/// <summary>
		/// Checks if the specified key is loaded, if not 
		/// the key hierachy is reconstructed and loaded.
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="keyContext">Specifies the context in which an already loaded key can be used.
		/// If a key has already been loaded in another context it is not allowed to use this key</param>
		public void LoadKey(string identifier, object keyContext, IKeyManagerHelper keymanagerHelper, ICommandAuthorizationHelper commandAuthHelper)
		{
			KeyHandleItem keyHandleItem;
			
			lock(_keyHandles)
			{
				keyHandleItem = _keyHandles.FindKeyHandleItem(identifier, keyContext);
			}
			
			if(keyHandleItem == null)
			{
				InternalLoadKey(identifier, keyContext, keymanagerHelper, commandAuthHelper);
			}
			else
			{
				//If the key was loaded and is currently Swapped out it would get 
				//loaded by an outside call of IdentifierToHandle, but if the keyHandle
				//was not used for a long time or of many key handles where loaded in the meantime,
				//it is neccessary to re-authorize the key handle (call Load Key again)
				//This would block the entire Keymanager for a long time because the secrets are requested
				//from the client so it is done here where the keymanager is (normally) not locked
				IdentifierToHandle(identifier, keyContext, keymanagerHelper);			
			}
		}
		
		/// <summary>
		/// Recursivly loads the specified key hierachy and returns the identifier of the parent key
		/// </summary>
		/// <param name="identifier">identifier of the key whos parent should be loaded</param>
		/// <param name="keyContext"></param>
		/// <param name="keystore"></param>
		/// <returns></returns>
		private string InternalLoadKey(string identifier, object keyContext, IKeyManagerHelper keymanagerHelper, ICommandAuthorizationHelper commandAuthHelper)
		{
			if(identifier == KeyHandle.KEY_SRK)
				return null;			   
			
			//SRK
			if(identifier == null)
				return null;
			
			if(keymanagerHelper.ContainsIdentifier(identifier) == false)
				throw new ArgumentException(string.Format("Keystore does not contain key with identifier: '{0}'", identifier));
				
			
			bool reloadKey = true;
			KeyHandleItem keyHandleItem = null;
			
			while(reloadKey)
			{
				lock(_keyHandles)
				{
					keyHandleItem = _keyHandles.FindKeyHandleItem(identifier, keyContext);
				
					
					if(keyHandleItem == null)
					{	
						//If no key handle with such an identifier exists, add a NotLoaded entry,
						//so other threads can wait for that handle to become ready
						_keyHandles.AddKeyHandle(new KeyHandleItem(new KeyHandle(identifier, 0),
							KeyHandleItem.KeyHandleStatus.NotLoaded, keyContext));
					}
					
				}
			
				reloadKey = false;
				//The key is currently being loaded, wait till the loading process has finished/or failed
				if(keyHandleItem != null && keyHandleItem.Status == KeyHandleItem.KeyHandleStatus.NotLoaded)
				{
					keyHandleItem.LoadedEvt.WaitOne();
					reloadKey = true;
				}
			}
			
			//Key is already loaded/swaped out
			if(keyHandleItem != null)
			{
				IdentifierToHandle(identifier, keyContext, keymanagerHelper);
				return identifier;
			}
			
		
		
			string parentKey = keymanagerHelper.FindParentKey(identifier);
			InternalLoadKey(parentKey, keyContext, keymanagerHelper, commandAuthHelper);
			
			Parameters paramLoadKey = new Parameters();
			
			// Load the key
			
			//SRK
			if(parentKey == null || parentKey == KeyHandle.KEY_SRK)
			{
				paramLoadKey.AddPrimitiveType("parent_key_srk", true);
			}
			else
			{
				paramLoadKey.AddPrimitiveType("parent_key_srk", false);
				//paramLoadKey.AddPrimitiveType("parent_handle", IdentifierToHandle(parentKey, keyContext, keymanagerHelper));
				paramLoadKey.AddPrimitiveType("parent_identifier", parentKey);
			}
				
			paramLoadKey.AddPrimitiveType("key_identifier", identifier);
			paramLoadKey.AddPrimitiveType("key_blob", keymanagerHelper.GetKeyBlob(identifier));
				
			TPMCommandRequest requestLoadKey = new TPMCommandRequest(TPMCommandNames.TPM_CMD_LoadKey2, paramLoadKey);
			TPMCommandResponse responseLoadKey = _tpmContext.TPM.Process(requestLoadKey, commandAuthHelper, keymanagerHelper);
			
			if(responseLoadKey.Status == false)
			{
				KeyHandleItem keyHandleToRemove = _keyHandles.FindKeyHandleItem(identifier, keyContext);
				if(keyHandleToRemove != null)
				{
					keyHandleItem.LoadedEvt.Set();
					_keyHandles.RemoveKeyHandle(keyHandleToRemove);
				}
				throw new TPMRequestException("Unknown error on running TPM_LoadKey2");
			}
			
			lock(_keyHandles)
			{		
				//Make sure that a dummy (not loaded) keyhandleitem get removed
				KeyHandleItem keyHandleToRemove = _keyHandles.FindKeyHandleItem(identifier, keyContext);
				if(keyHandleToRemove != null)
				{
					keyHandleToRemove.LoadedEvt.Set();
					_keyHandles.RemoveKeyHandle(keyHandleToRemove);
				}
							
				KeyHandleItem item = new KeyHandleItem(responseLoadKey.Parameters.GetValueOf<KeyHandle>("handle"),
					KeyHandleItem.KeyHandleStatus.SwappedIn, keyContext);
				_keyHandles.AddKeyHandle(item);
					
				AddNewItem(item);
			}
				
												
				
			return parentKey;
		}

		/// <summary>
		/// Returns the tpm handle of the given key identifier,
		/// The key must be loaded before. If the key is swapped out it gets swapped in.
		/// AcquireLock should be called outside, otherwise it is possible that the key handle gets swapped out by another
		/// thread
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="keyContext">Specifies the context in which an already loaded key can be used.
		/// If a key has already been loaded in another context it is not allowed to use this key</param>
		/// <returns></returns>
		public KeyHandle IdentifierToHandle(string identifier, object keyContext, IKeyManagerHelper keymanagerHelper)
		{
			lock(_keyHandles)
			{
				KeyHandleItem keyHandleItem = _keyHandles.FindKeyHandleItem(identifier, keyContext);
			
				
				if(keyHandleItem == null)
				{	
					throw new KeyNotFoundException(string.Format(@"Keymanager could not find loaded key with identifier
						'{0}'", identifier));
				}
				
				if(keyHandleItem.Status == KeyHandleItem.KeyHandleStatus.SwappedOut)
				{
					SwapIn(keyHandleItem);
				}
				
				_replacementAlgorithm.RegisterUsed(ItemToId(keyHandleItem).Value);
				
				return keyHandleItem.KeyHandle;
				
			}
		}
		
		
		/// <summary>
		/// Acquires an exclusive key manager lock
		/// </summary>
		/// <returns></returns>
		public LockContext AcquireLock()
		{
			return new LockContext(_lockTarget);
		}
		
		/// <summary>
		/// Ensures that a free key slot is available
		/// </summary>
		public void EnsureFreeSlot()
		{
			while(LoadedKeys >= AvailableKeySlots)
				SwapOut();
		}

	}
	
	public class KeyHandleCollection : IEnumerable<KeyHandleItem>
	{
		/// <summary>
		/// Contains all current authorization handles
		/// </summary>
		List<KeyHandleItem> _keyHandles = new List<KeyHandleItem>();
		
		/// <summary>
		/// Adds the specified auth handle item to the list
		/// </summary>
		/// <param name="authHandleItem"></param>
		public void AddKeyHandle(KeyHandleItem keyHandleItem)
		{
			_keyHandles.Add(keyHandleItem);
		}
		
		/// <summary>
		/// Removes the specified authhandle
		/// </summary>
		/// <param name="tpmSessionIdentifier"></param>
		/// <param name="authHandleItem"></param>
		public void RemoveKeyHandle(KeyHandleItem keyHandleItem)
		{
			_keyHandles.Remove(keyHandleItem);
		}
		
		
		/// <summary>
		/// Finds a KeyHandleItem for the specified identifier
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns> </returns>
		public KeyHandleItem FindKeyHandleItem(string identifier, object keyContext)
		{
			foreach(KeyHandleItem keyHandleItem in _keyHandles)
			{
				if(keyHandleItem.KeyHandle.Identifier == identifier
				  && keyHandleItem.KeyContext == keyContext)
					return keyHandleItem;
			}
			
			return null;
		}
		
		
		/// <summary>
		/// Finds the key handles with the specified status
		/// </summary>
		/// <remarks>
		/// The returned list is sorted by last used time
		/// </remarks>
		/// <param name="status"></param>
		/// <returns></returns>
		public List<KeyHandleItem> FindKeyHandles(KeyHandleItem.KeyHandleStatus status)
		{
			List<KeyHandleItem> items = new List<KeyHandleItem>();
			foreach(KeyHandleItem handle in _keyHandles)
			{
				if(handle.Status == status)
					items.Add(handle);
			}
				
			items.Sort(delegate(KeyHandleItem left, KeyHandleItem right)
				{
					return left.LastUsed.CompareTo(right.LastUsed);
				}
			);
			
			return items;
			
		}
		
		
		#region IEnumerable<KeyHandleItem> implementation
		public IEnumerator<KeyHandleItem> GetEnumerator ()
		{
			return _keyHandles.GetEnumerator();
		}
		
		#endregion
		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return _keyHandles.GetEnumerator();
		}
		
		#endregion
	}
	
	public class KeyHandleItem
	{
		public enum KeyHandleStatus
		{
			/// <summary>
			/// Key handle is loaded and can be used
			/// </summary>
			SwappedIn,
			
			/// <summary>
			/// Key handle is swapped out, swap in to use it
			/// </summary>
			SwappedOut,
			
			/// <summary>
			/// Key handle needs to be created
			/// </summary>
			NotLoaded
		}
		
		private ManualResetEvent _loadedEvt = new ManualResetEvent(false);
		
		public ManualResetEvent LoadedEvt
		{
			get{ return _loadedEvt; }
		}
		
		
		private object _keyContext;
		
		public object KeyContext
		{
			get{ return _keyContext; }
		}
		
		private int _lastUsed = Environment.TickCount;
		
		/// <summary>
		/// Returns the ticks the handle was last used
		/// </summary>
		public int LastUsed
		{
			get{  return _lastUsed; }
		}
		
				
		private KeyHandleStatus _status = KeyHandleStatus.NotLoaded;
		
		/// <summary>
		/// Returns the Swap/Load state of this Authorization handle
		/// </summary>
		public KeyHandleStatus Status
		{
			get{ return _status; }
			set{ _status = value;}
		}
		
		private KeyHandle _keyHandle;
		
		/// <summary>
		/// Gets the TPM specific key handle
		/// </summary>
		public KeyHandle KeyHandle
		{
			get{ return _keyHandle; }
		}
		
		public KeyHandleItem(KeyHandle keyHandle, KeyHandleStatus status, object keyContext)
		{
			_keyHandle = keyHandle;
			_status = status;
			_keyContext = keyContext;
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
