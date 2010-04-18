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
using Iaik.Utils.Replacement;
using System.Security.Cryptography;

namespace tpm_test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
//			RSA rsa = new RSACryptoServiceProvider (2048);
//			RSAParameters parameters = rsa.ExportParameters (false);
//			
//			Console.WriteLine ("P: {0}\nQ: {1}\nD: {2}\nDP: {3}\nDQ: {4}\nExpo: {5}\nInvQ: {6}\nMod: {7}", 
//				ByteHelper.ByteArrayToHexString (parameters.P),
//				ByteHelper.ByteArrayToHexString (parameters.Q),
//				ByteHelper.ByteArrayToHexString (parameters.D),
//				ByteHelper.ByteArrayToHexString (parameters.DP),
//				ByteHelper.ByteArrayToHexString (parameters.DQ),
//				ByteHelper.ByteArrayToHexString (parameters.Exponent),
//				ByteHelper.ByteArrayToHexString (parameters.InverseQ),
//				ByteHelper.ByteArrayToHexString (parameters.Modulus));
//			
//			rsa = new RSACryptoServiceProvider ();
//			rsa.ImportParameters (parameters);
//			rsa.EncryptValue (new byte[] { 0, 1, 2, 3 });
			
			SetupLogging();
//			TPMWrapper tpm = new TPMWrapper();
//			IDictionary<string, string> dict = new Dictionary<string, string>();
//			dict.Add("DeviceName","/dev/tpm0");
//			dict.Add("debug", "True");
//			tpm.Init("linux/device", dict);
//			//tpm.Init("linux/tddl", dict);
//			tpm.Open();
//			
//			ReadPCRs(tpm);
//			ReadCapabilities(tpm);
//			//EstablishOIAP(tpm);
//			tpm.Dispose();
//			//tpm.init;
//			//tpm.backend.tpmOpen();
		
			TestAging();
		}
		
		private static void ReadPCRs(TPMWrapper tpm){
			uint i = 0;
					
		    ILog log = LogManager.GetLogger("ReadPCRs");
			
			Parameters param = new Parameters();
			param.AddPrimitiveType("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY);
			param.AddPrimitiveType("subCap", CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_PCR);
			TPMCommandRequest request = new TPMCommandRequest(TPMCommandNames.TPM_CMD_GetCapability, param);
			TPMCommandResponse response = tpm.Process(request);
			
			uint maxPcrs = response.Parameters.GetValueOf<uint>(CapabilityData.PARAM_PROP_PCR);
			
			for(i=0; i<maxPcrs; ++i){
			
				param = new Parameters();
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
		
		private static void ReadCapabilities(TPMWrapper tpm)
		{
			ILog log = LogManager.GetLogger("ReadCapabilities");
			// Read out each capability property
			foreach(CapabilityData.TPMSubCapProperty subCap in Enum.GetValues(typeof(CapabilityData.TPMSubCapProperty)))
			{
				Parameters param = new Parameters();
				param.AddPrimitiveType("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY);
				param.AddPrimitiveType("subCap", subCap);
				TPMCommandRequest request = new TPMCommandRequest(TPMCommandNames.TPM_CMD_GetCapability, param);
				TPMCommandResponse response = tpm.Process(request);
				switch(subCap)
				{
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_PCR:
					log.InfoFormat("Number of PCRs is {0}", response.Parameters.GetValueOf<uint>(CapabilityData.PARAM_PROP_PCR));
					break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_DIR:
					log.InfoFormat("Number of DIR is {0} and should be 1 because command is deprecated", response.Parameters.GetValueOf<uint>(CapabilityData.PARAM_PROP_DIR));
					break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MANUFACTURER:
					log.InfoFormat("Manufacturer ID is {0}", response.Parameters.GetValueOf<uint>(CapabilityData.PARAM_PROP_MANUFACTURER));
					break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_KEYS:
					log.InfoFormat("Number of free keyslots is {0}", response.Parameters.GetValueOf<uint>(CapabilityData.PARAM_PROP_KEYS));
					break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_AUTHSESS:
					log.InfoFormat("Number of max Auth Sessions is {0}", response.Parameters.GetValueOf<uint>(CapabilityData.PARAM_PROP_MAX_AUTHSESS));
					break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_TRANSESS:
					log.InfoFormat("Number of max Trans Sessions is {0}", response.Parameters.GetValueOf<uint>(CapabilityData.PARAM_PROP_MAX_TRANSESS));
					break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_KEYS:
					log.InfoFormat("Number of max keys (without EK and SRK) is {0}", response.Parameters.GetValueOf<uint>(CapabilityData.PARAM_PROP_MAX_KEYS));
					break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_SESSIONS:
					log.InfoFormat("Number of max Sessions is {0}", response.Parameters.GetValueOf<uint>(CapabilityData.PARAM_PROP_MAX_SESSIONS));
					break;
				default:
					throw new NotSupportedException("Defined cap or subcap are not supported");
					break;
				}
			}
			
			
		}
		
		private static void TestAging()
		{
			ILog log = LogManager.GetLogger("TestAging");
			IReplacementAlgorithm alg = new Aging();
			List<UInt64> li = new List<UInt64>();
			for(int i=0; i<8;++i)
				li.Add(alg.RegisterNew());
			
			List<UInt64> up = new List<UInt64>();
			up = alg.Swapables;
			up = new List<UInt64>();
			up.Add(1);
			up.Add(4);
			alg.Update(up);
			up = alg.Swapables;
//			foreach(UInt64 id in up)
//				log.Info(id);
//			log.Info("------------------");
			up = new List<UInt64>();
			up.Add(3);
			up.Add(5);
			alg.Update(up);
			up = alg.Swapables;
//			foreach(UInt64 id in up)
//				log.Info(id);
//			log.Info("------------------");
			up = new List<UInt64>();
			up.Add(0);
			alg.Update(up);
			up = alg.Swapables;
//			foreach(UInt64 id in up)
//				log.Info(id);
//			log.Info("------------------");
			for(int i=0; i<8;++i)
				alg.Update(null);
			for(int i=0; i<8;++i)
			{
				alg.Update(null);
				up = new List<UInt64>();
				up.Add((UInt64)i);
				alg.Update(up);
			}
			
			alg.SwapOut(1);
			alg.SwapOut(5);
			alg.SwapOut(7);
			alg.Update(null);
			up = alg.Swapables;
				foreach(UInt64 id in up)
					log.Info(id);
				log.Info("------------------");
			alg.SwapIn(7);
			alg.Update(null);
			up = alg.Swapables;
				foreach(UInt64 id in up)
					log.Info(id);
				log.Info("------------------");
			up = new List<UInt64>();
				up.Add(1);
			up.Add(5);
			alg.SwapIn(up);
			alg.Update(null);
			up = new List<UInt64>();
			up.Add(0);
			alg.Update(up);
						
			up = alg.Swapables;
				foreach(UInt64 id in up)
					log.Info(id);
				log.Info("------------------");
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
