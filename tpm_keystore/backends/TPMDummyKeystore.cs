// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Keystore.Backends
{

	/// <summary>
	/// This class 
	/// </summary>
	[TPMKeystore("dummykeystore")]
	public class TPMDummyKeystore : TPMKeystoreProvider
	{

		public TPMDummyKeystore ()
		{
		
		}
	}
}
