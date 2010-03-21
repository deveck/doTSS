
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
		private TPMProvider _tpmProvider;
		
		
		public TPMContext (string deviceName, TPMProvider tpmProvider)
		{
			_deviceName = deviceName;
			_tpmProvider = tpmProvider;
		}
	}
}
