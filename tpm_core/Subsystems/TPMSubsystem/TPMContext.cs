
using System;
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common.KeyData;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{

	/// <summary>
	/// Combines all tpm provider related objects (tpm provider, Key storage, ...)
	/// </summary>
	public class TPMContext
	{
		private string _deviceName;
		
		/// <summary>
		/// Gets the configuration device name of this tpm device
		/// </summary>
		public string DeviceName
		{
			get { return _deviceName; }
		}
		
		private TPMWrapper _tpm;

		/// <summary>
		/// Gets access to the TPM device
		/// </summary>
		public TPMWrapper TPM
		{
			get { return _tpm;}
		}
		
		private IAuthHandleManager _authHandleManager;
		
		/// <summary>
		/// Manages and swaps the authorization handles as needed
		/// </summary>
		public IAuthHandleManager AuthHandleManager
		{
			get{ return _authHandleManager; }
		}
		
		private IKeyManager _keyManager;
		
		/// <summary>
		/// Manages and swapt the key handles as needed
		/// </summary>
		public IKeyManager KeyManager
		{
			get{ return _keyManager; }	
		}
		
		public TPMContext (string deviceName, TPMWrapper tpm)
		{
			_deviceName = deviceName;
			_tpm = tpm;
			_authHandleManager = new AuthHandleManager(this);
			_keyManager = new KeyManager(this);
		}
	}
}
