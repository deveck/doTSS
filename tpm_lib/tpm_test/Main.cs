// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
using System;
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Commands;
using Iaik.Tc.TPM.Lowlevel;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using log4net;
using Iaik.Utils;

namespace tpm_test
{
	class MainClass
	{
		public static void Main (string[] args)
		{				
			SetupLogging();
			TPM tpm = new TPM();
			IDictionary<string, string> dict = new Dictionary<string, string>();
			dict.Add("DeviceName","/dev/tpm0");
			dict.Add("debug", "True");
			tpm.Init("linux/device", dict);
			tpm.Open();
			
			ReadPCRs(tpm);
			
			tpm.Dispose();
			//tpm.init;
			//tpm.backend.tpmOpen();
			
		}
		
		private static void ReadPCRs(TPM tpm){
			UInt32 i = 0;
			
		    ILog log = LogManager.GetLogger("ReadPCRs");
			
			for(i=0; i<24; ++i){
			
				Parameters param = new Parameters();
				param.AddPrimitiveType("pcrnum", i);
				TPMCommandRequest req = new TPMCommandRequest(TPMCommandNames.TPM_CMD_PCRRead,param);
				TPMCommandResponse resp = tpm.Process(req);
				
				byte[] val = resp.Parameters.GetValueOf<byte[]>("value");
				
				log.InfoFormat("Answer for PCR {0} is: 0x{1}", resp.Parameters.GetValueOf<UInt32>("pcrnum"),
				               ByteHelper.ByteArrayToHexString(val));
			}
			
//			TPMCommandRequest req = new TPMCommandRequest(TPMCommandNames.TPM_CMD_PCRRead, null);
//			TPMCommand com = TPMCommandFactory.Create(req);
//			com.Init(param, tpm);
//			com.Process();
			//Console.WriteLine ("Hello World!");
		}
		
		
		/// <summary>
		/// Initializes the logger
		/// </summary>
		private static void SetupLogging()
		{
			
			log4net.Appender.ConsoleAppender appender = new log4net.Appender.ConsoleAppender();
			appender.Name = "ConsoleAppender";
			appender.Layout = new log4net.Layout.PatternLayout("[%date{dd.MM.yyyy HH:mm:ss,fff}]-%-5level-[%c]: %message%newline");
			log4net.Config.BasicConfigurator.Configure(appender);
		}
	}
}
