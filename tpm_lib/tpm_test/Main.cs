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
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common.Handles;
using Iaik.Utils.Hash;
using System.Security.Cryptography;

namespace tpm_test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			RSA rsa = new RSACryptoServiceProvider (2048);
			RSAParameters parameters = rsa.ExportParameters (false);
			
			Console.WriteLine ("P: {0}\nQ: {1}\nD: {2}\nDP: {3}\nDQ: {4}\nExpo: {5}\nInvQ: {6}\nMod: {7}", 
				ByteHelper.ByteArrayToHexString (parameters.P),
				ByteHelper.ByteArrayToHexString (parameters.Q),
				ByteHelper.ByteArrayToHexString (parameters.D),
				ByteHelper.ByteArrayToHexString (parameters.DP),
				ByteHelper.ByteArrayToHexString (parameters.DQ),
				ByteHelper.ByteArrayToHexString (parameters.Exponent),
				ByteHelper.ByteArrayToHexString (parameters.InverseQ),
				ByteHelper.ByteArrayToHexString (parameters.Modulus));
			
			rsa = new RSACryptoServiceProvider ();
			rsa.ImportParameters (parameters);
			rsa.EncryptValue (new byte[] { 0, 1, 2, 3 });
			
			
			SetupLogging();
			TPMWrapper tpm = new TPMWrapper();
			IDictionary<string, string> dict = new Dictionary<string, string>();
			dict.Add("DeviceName","/dev/tpm0");
			dict.Add("debug", "True");
			//tpm.Init("linux/device", dict);
			tpm.Init("linux/tddl", dict);
			tpm.Open();
			
			//ReadPCRs(tpm);
			EstablishOIAP(tpm);
			tpm.Dispose();
			//tpm.init;
			//tpm.backend.tpmOpen();
			
		}
		
		private static void ReadPCRs(TPMWrapper tpm){
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
		
		
		private static AuthHandle EstablishOIAP (TPMWrapper tpm)
		{
			ILog log = LogManager.GetLogger ("EstablishOIAP");

			for (int i = 0; i < 2; i++)
			{
				TPMCommandRequest request = new TPMCommandRequest (TPMCommandNames.TPM_CMD_OIAP, new Parameters ());
				TPMCommandResponse response = tpm.Process (request);
			
				AuthHandle myAuthHandle = response.Parameters.GetValueOf<AuthHandle> ("auth_handle");
				
//				Parameters parameters = new Parameters ();
				//				parameters.AddValue ("handle", myAuthHandle);
				//				TPMCommandRequest requestFlush = new TPMCommandRequest (TPMCommandNames.TPM_CMD_FLUSH_SPECIFIC, parameters);
				//				tpm.Process (requestFlush);
				
			//return myAuthHandle;
			}
			
			Parameters listHandlesParameters = new Parameters ();
			listHandlesParameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_HANDLE);
			listHandlesParameters.AddPrimitiveType ("handle_type", TPMResourceType.TPM_RT_AUTH);
			
			TPMCommandRequest listHandlesRequest = new TPMCommandRequest (TPMCommandNames.TPM_CMD_GetCapability, listHandlesParameters);
			TPMCommandResponse listHandlesResponse = tpm.Process (listHandlesRequest);
			
			HandleList loadedKeyHandles = listHandlesResponse.Parameters.GetValueOf<HandleList> ("handles");
			
			
			
			return null;
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
