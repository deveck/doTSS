// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Keystore
{

	public abstract class TPMKeystoreProvider : IDisposable
	{
		public TPMKeystoreProvider ()
		{
		}
		
		#region IDisposable implementation
		void  IDisposable.Dispose ()
		{
			throw new System.NotImplementedException();
		}
		#endregion
		
		
		
	}
}
