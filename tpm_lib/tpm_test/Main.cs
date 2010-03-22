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
			TPM tpm = new TPM();
			tpm.Init("linux/tddl");
			tpm.Open();
			
			ReadPCRs(tpm);
			//tpm.init;
			//tpm.backend.tpmOpen();
			
		}
		
		private static void ReadPCRs(TPM tpm){
			UInt32 i = 0;
			
			for(i=0; i<24; ++i){
			
				Parameters param = new Parameters();
				param.AddPrimitiveType("pcrnum", i);
				TPMCommandRequest req = new TPMCommandRequest(TPMCommandNames.TPM_CMD_PCRRead,param);
				TPMCommandResponse resp = tpm.Process(req);
				
				byte[] val = resp.Parameters.GetValueOf<byte[]>("value");
				Console.Write("Answer for PCR {0} is: 0x", resp.Parameters.GetValueOf<UInt32>("pcrnum"));
				foreach(byte b in val)
					Console.Write(String.Format("{0:X2}",b));
				Console.WriteLine();
			}
			
//			TPMCommandRequest req = new TPMCommandRequest(TPMCommandNames.TPM_CMD_PCRRead, null);
//			TPMCommand com = TPMCommandFactory.Create(req);
//			com.Init(param, tpm);
//			com.Process();
			//Console.WriteLine ("Hello World!");
		}
	}
}
