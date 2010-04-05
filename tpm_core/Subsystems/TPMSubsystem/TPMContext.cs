
using System;
using TPMWrapper = Iaik.Tc.TPM.Library.TPM;

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
		
		
		
		public TPMContext (string deviceName, TPMWrapper tpm)
		{
			_deviceName = deviceName;
			_tpm = tpm;
		}
	}
}
