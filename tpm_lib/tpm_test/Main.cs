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
using Iaik.Utils.SwapUtils;
using Iaik.Tc.TPM.Keystore;

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
		
			//TestAging();
			//TestHMAC();
			TestKeystoreSqlite();
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
			alg.RegisterUsed(up);
			alg.Update();
			up = alg.Swapables;
			foreach(UInt64 id in up)
				log.Info(id);
			log.Info("------------------");
			up = new List<UInt64>();
			up.Add(3);
			up.Add(5);
			alg.RegisterUsed(up);
			alg.Update();
			up = alg.Swapables;
			foreach(UInt64 id in up)
				log.Info(id);
			log.Info("------------------");
			up = new List<UInt64>();
			up.Add(0);
			alg.RegisterUsed(up);
			alg.Update();
			up = alg.Swapables;
			foreach(UInt64 id in up)
				log.Info(id);
			log.Info("------------------");
			for(int i=0; i<8;++i)
				alg.Update();
			log.Info("------------------");
			for(ulong i=0; i<8;++i)
			{
				alg.Update();
				alg.RegisterUsed(i);
				alg.Update();
			}
			up = alg.Swapables;
			foreach(UInt64 id in up)
				log.Info(id);
			log.Info("------------------");
			
			alg.SwapOut(1);
			alg.SwapOut(5);
			alg.SwapOut(7);
			alg.Update();
			up = alg.Swapables;
				foreach(UInt64 id in up)
					log.Info(id);
				log.Info("------------------");
			alg.SwapIn(7);
			alg.Update();
			up = alg.Swapables;
				foreach(UInt64 id in up)
					log.Info(id);
				log.Info("------------------");
			up = new List<UInt64>();
				up.Add(1);
			up.Add(5);
			alg.SwapIn(up);
			alg.Update();
			alg.RegisterUsed(0);
			alg.Update();
						
			up = alg.Swapables;
				foreach(UInt64 id in up)
					log.Info(id);
				log.Info("------------------");
		}
		
		public enum testenum: ushort
		{
			test = 0x14
		}
		private static void TestHMAC()
		{
			ILog log = LogManager.GetLogger("TestHMAC");
			
			HashProvider hash = new HashProvider();
			byte[] h1 = hash.Hash(
			   new HashPrimitiveDataProvider((uint)0x3c),
			   new HashPrimitiveDataProvider((ushort)0x00),
			   new HashEnumDataProvider(testenum.test),
			   new HashByteDataProvider(new byte[]{0xb3,0xd5,0xcb, 0x12,0x73,
			0x8b, 0xb6, 0xf9, 0x21, 0xa3,
			0xda, 0x42,0xe0, 0x18, 0xd1,
			0x43, 0xfa, 0x29, 0x7c, 0xa6}));
			  
			
			HMACProvider hmac = new HMACProvider(
			   new byte[]{0x75, 0xf0, 0x86, 0x84, 0x78, 
				0x24, 0xf8, 0x79, 0x39, 0x5a,
				0x18, 0x14, 0x1d, 0x19, 0x0c,
				0x2f, 0x01, 0x29, 0x0b, 0x05});
			
			byte[] h2 = new byte[20];
			for(int i = 0; i<20; i++)
				h2[i] = 0xa5;
			
			byte[] h3 = new byte[]{
				0xb9, 0x73, 0x05, 0xfa, 0xdb,
				0xe3, 0x4d, 0xc5, 0x46, 0x65,
				0x10, 0x00, 0x0a, 0x55, 0x04,
				0x2e, 0x3f, 0xea, 0xbf, 0x27};
			 
			byte[] result = hmac.Hash(new HashByteDataProvider(h1),
			          new HashByteDataProvider(h2),
			          new HashByteDataProvider(h3),
			          new HashPrimitiveDataProvider(true));
			     
			byte[] expected = new byte[]{
				0x26, 0x7e, 0xca, 0x16, 0xa1,
				0x4d, 0x36, 0xe6, 0x72, 0x2e,
				0xaa, 0x7f, 0x7b, 0x53, 0x4a,
				0xb3, 0xce, 0x8b, 0x2a, 0xaa};
			
			for(int i = 0; i<20; i++)
			{
				if(result[i] != expected[i])
					Console.WriteLine("FAILED");
			}
			
			Console.WriteLine("SUCCESS");
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
		
		private static void TestKeystoreSqlite()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("file", "test.db");
			
			using(TPMKeystoreProvider keystore = TPMKeystoreProviders.Create("SQLiteKeystore", parameters))
			{
				keystore.AddKey("FriendlyName1", "ident1", null, new byte[]{0,1,2,3,4});
			}
		}
	}
}
