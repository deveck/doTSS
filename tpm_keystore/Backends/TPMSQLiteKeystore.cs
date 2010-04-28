// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Keystore.Backends
{

	[TPMKeystore("SQLiteKeystore")]
	public class TPMSQLiteKeystore : TPMKeystoreProvider
	{

		public TPMSQLiteKeystore ()
		{
		}
	}
}
