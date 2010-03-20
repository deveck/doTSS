
using System;
using Iaik.Tc.Tpm.lowlevel;

namespace Iaik.Tc.Tpm.library
{

	/// <summary>
	/// Combines all tpm provider related objects (tpm provider, Key storage, ...)
	/// </summary>
	public class TpmContext
	{
		private string _deviceName;
		private TPMProvider _tpmProvider;
		
		
		public TpmContext (string deviceName, TPMProvider tpmProvider)
		{
			_deviceName = deviceName;
			_tpmProvider = tpmProvider;
		}
	}
}
