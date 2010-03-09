// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.lowlevel;

namespace Iaik.Tc.Tpm.library
{


	public class TPM
	{
		
		private TPMProvider backend_;
		

		public TPM(){}
		public void init (string providerName)
		{
			backend_ = TpmProviders.Create(providerName,null);
		}
	}
}
