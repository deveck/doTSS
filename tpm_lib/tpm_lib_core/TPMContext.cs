
using System;
using Iaik.Tc.TPM.Lowlevel;

namespace Iaik.Tc.TPM.Library
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
		
		private TPM _tpm;

		/// <summary>
		/// Gets access to the TPM device
		/// </summary>
		public TPM TPM
		{
			get { return _tpm;}
		}
		
		
		
		public TPMContext (string deviceName, TPM tpm)
		{
			_deviceName = deviceName;
			_tpm = tpm;
		}
	}
}
