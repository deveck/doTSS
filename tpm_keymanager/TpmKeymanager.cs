// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Mono.Security.X509;
using Iaik.Tc.TPM.Library;

namespace Iaik.Tc.TPM.Keymanager
{


	public class TpmKeymanager : IDisposable
	{
		TPMWrapper _tpm = null;
		
		bool _isDisposed = false;
		
		private TpmKeymanager ()
		{
			
		}
		
		public TpmKeymanager (TPMWrapper tpm)
		{
			_tpm = tpm;
		}
		
		
		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			lock(this)
				if(_isDisposed)
					throw new ObjectDisposedException("TPMKeymanager object is disposed");
			_tpm.Dispose();
			_isDisposed = true;
		}
		
		#endregion

		
	}
}
