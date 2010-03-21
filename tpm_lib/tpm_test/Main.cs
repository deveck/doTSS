// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
using System;
using Iaik.Tc.Tpm.library;
using Iaik.Tc.Tpm.library.common;
using Iaik.Tc.Tpm.library.commands;
using Iaik.Tc.Tpm.lowlevel;

namespace tpm_test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			TPMProvider tpm = TpmProviders.Create("linux/tddl",null);
			
			tpm.Open();
			//tpm.init;
			//tpm.backend.tpmOpen();
			UInt32 i = 1;
			
			Parameters param = new Parameters();
			param.AddPrimitiveType("pcrnum", i);
			TPMCommandRequest req = new TPMCommandRequest(TPMCommandNames.TPM_CMD_PCRRead, null);
			TpmCommand com = TpmCommandFactory.Create(req);
			com.Init(param, tpm);
			com.Process();
			Console.WriteLine ("Hello World!");
		}
	}
}
