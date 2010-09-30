// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
using System;
using Iaik.Tc.TPM.Library.Common.PCRData;
using Iaik.Utils.Hash;
using System.Collections.Generic;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Org.BouncyCastle.Crypto;
using Iaik.Tc.TPM.Configuration.ClientConfiguration;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Keystore.Backends;
using Iaik.Utils;
using Iaik.Tc.TPM.Subsystems.TPMSubsystem;

namespace SealUnseal
{
	class MainClass
	{
		const string base_path = "/home/nn/git-repos/TU/TPM/IAIK-Repos/src/";
		
		public static void Main (string[] args)
		{
			string[] sealMe = {"Hallo", "IAIK!"};
						
			// Establish Connections
			IDictionary<string, TPMSession> sessions =
				XMLConfiguration.EstablischConnection(base_path + "Examples/ClientConfigXml/UnixSocketDeviceLin.xml"); 
			
			// Create one keystore per opened session
			foreach (TPMSession tpmSes in sessions.Values)
				tpmSes.Keystore = new InMemoryKeystore();
			
			TPMSession sessionToUse = sessions["local0"];
			
			sessionToUse.SetRequestSecretCallback(RequestSecret);
			
			Console.WriteLine("Create Cipher Key");
						
			ClientKeyHandle myFirstSealKey = 
				sessionToUse.KeyClient.GetSrkKeyHandle().CreateKey("my_first_seal_key", TPMKeyUsage.TPM_KEY_STORAGE);
			
			Console.WriteLine("Key: {0}\n{1}", myFirstSealKey.FriendlyName, myFirstSealKey.PublicKey);
			Console.WriteLine("---------------------------------\n");

			
			sessionToUse.IntegrityClient.Extend(0, new byte[]{0,1,2,3,4,5,6,7,8,9,9,8,7,6,5,4,3,2,1,0});
			sessionToUse.IntegrityClient.Extend(1, new byte[]{0,1,2,3,4,5,6,7,8,9,9,8,7,6,5,4,3,2,1,0});
			sessionToUse.IntegrityClient.Extend(2, new byte[]{0,1,2,3,4,5,6,7,8,9,9,8,7,6,5,4,3,2,1,0});
			
			TPMPCRSelection pcrselect = sessionToUse.CreateEmptyPCRSelection();
			
			pcrselect.PcrSelection[0] = true;
			pcrselect.PcrSelection[1] = true;
			pcrselect.PcrSelection[2] = true;
			
			Console.WriteLine("Create Cipher, init and cipher");
			IAsymmetricBlockCipher cipher = myFirstSealKey.CreateSealBlockCipher(pcrselect);
			cipher.Init(true, null);
			
			byte[][] cipherText = new byte[sealMe.Length][];
			int i=0;
			
			foreach(string msg in sealMe)
			{
				byte[] block = System.Text.ASCIIEncoding.ASCII.GetBytes(msg);
				cipherText[i] = cipher.ProcessBlock(block, 0, block.Length);
				i++;
			}

			Console.WriteLine("Original vs. CiperText:");
			for(i=0; i<sealMe.Length; i++)
				Console.WriteLine("{0} --> {1}", sealMe[i], ByteHelper.ByteArrayToHexString(cipherText[i]));
			Console.WriteLine("---------------------------------\n");
			
			Console.WriteLine("Init and decode");
			cipher.Init(false, null);
			byte[][] decode = new byte[sealMe.Length][];
			i=0;
			foreach(byte[] msg in cipherText)
			{
				decode[i] = cipher.ProcessBlock(msg, 0, msg.Length);
				i++;
			}
			
			Console.WriteLine("Does it work?:");
			for(i=0; i<sealMe.Length; i++)
				Console.WriteLine("{0}: {1}", sealMe[i]==System.Text.ASCIIEncoding.ASCII.GetString(decode[i])?"Y":"N", System.Text.ASCIIEncoding.ASCII.GetString(decode[i]));
			Console.WriteLine("---------------------------------\n");
			
			Console.WriteLine("Changing PCR Values");
			sessionToUse.IntegrityClient.Extend(0, new byte[]{0,1,2,3,4,5,6,7,8,9,9,8,7,6,5,4,3,2,1,0});
			
			Console.WriteLine("Decode, now an TPMRequest Exception should be thrown, with Error Code (0x18): TPM_WRONGPCRVAL");
			decode = new byte[sealMe.Length][];
			i=0;
			foreach(byte[] msg in cipherText)
			{
				try
				{
					decode[i] = cipher.ProcessBlock(msg, 0, msg.Length);
					Console.WriteLine("UUUUUPPPPSSSS, something went wrong!");
				}
				catch(TPMRequestException e)
				{
					Console.WriteLine(e.ToString());
				}
				i++;
			}
		}
		
		static ProtectedPasswordStorage RequestSecret(HMACKeyInfo keyInfo)
		{
			if(keyInfo.KeyType == HMACKeyInfo.HMACKeyType.SrkSecret)
			{
				ProtectedPasswordStorage secret = new ProtectedPasswordStorage();
				secret.WellKnown();
				return secret;
			}
			
			ProtectedPasswordStorage pws = new ProtectedPasswordStorage();
			pws.AppendPasswordChar('I');
			pws.AppendPasswordChar('A');
			pws.AppendPasswordChar('I');
			pws.AppendPasswordChar('K');
			
			return pws;
			
		}
	}
}
