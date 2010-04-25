
using System;
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;

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
		
		public TPMContext (string deviceName, TPMWrapper tpm)
		{
			_deviceName = deviceName;
			_tpm = tpm;
			_authHandleManager = new AuthHandleManager(this);
		}
	}
}
