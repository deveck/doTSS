// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
using System;
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Commands;
using Iaik.Tc.TPM.Lowlevel;

namespace tpm_test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			TPMProvider tpm = TPMProviders.Create("linux/tddl",null);
			
			tpm.Open();
			//tpm.init;
			//tpm.backend.tpmOpen();
			UInt32 i = 1;
			
			Parameters param = new Parameters();
			param.AddPrimitiveType("pcrnum", i);
			TPMCommandRequest req = new TPMCommandRequest(TPMCommandNames.TPM_CMD_PCRRead, null);
			TPMCommand com = TPMCommandFactory.Create(req);
			com.Init(param, tpm);
			com.Process();
			Console.WriteLine ("Hello World!");
		}
	}
}
