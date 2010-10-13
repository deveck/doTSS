// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
using System;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Org.BouncyCastle.Crypto;
using Iaik.Tc.TPM.Library.Common.PCRData;
using System.Collections.Generic;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Configuration.ClientConfiguration;
using Iaik.Tc.TPM.Keystore.Backends;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils;

namespace BindUnbind
{
	class MainClass
	{
		const string base_path = "../../../";
		
		public static void Main (string[] args)
		{
			string[] bindMe = {"Hallo", "IAIK!"};
						
			// Establish Connections
			IDictionary<string, TPMSession> sessions =
				XMLConfiguration.EstablischConnection(base_path + "ClientConfigXml/UnixSocketDeviceLin.xml"); 
			
			// Create one keystore per opened session
			foreach (TPMSession tpmSes in sessions.Values)
				tpmSes.Keystore = new InMemoryKeystore();
			
			TPMSession sessionToUse = sessions["local0"];
			
			sessionToUse.SetRequestSecretCallback(RequestSecret);
			
			Console.WriteLine("Create Cipher Key");
						
			ClientKeyHandle myFirstBindKey = 
				sessionToUse.KeyClient.GetSrkKeyHandle().CreateKey("my_first_bind_key", TPMKeyUsage.TPM_KEY_BIND);
			
			Console.WriteLine("Key: {0}\n{1}", myFirstBindKey.FriendlyName, myFirstBindKey.PublicKey);
			Console.WriteLine("---------------------------------\n");

			
			Console.WriteLine("Create Cipher, init and cipher");
			IAsymmetricBlockCipher cipher = myFirstBindKey.CreateBindBlockCipher();
			cipher.Init(true, null);
			
			byte[][] cipherText = new byte[bindMe.Length][];
			int i=0;
			
			foreach(string msg in bindMe)
			{
				byte[] block = System.Text.ASCIIEncoding.ASCII.GetBytes(msg);
				cipherText[i] = cipher.ProcessBlock(block, 0, block.Length);
				i++;
			}

			Console.WriteLine("Original vs. CiperText:");
			for(i=0; i<bindMe.Length; i++)
				Console.WriteLine("{0} --> {1}", bindMe[i], ByteHelper.ByteArrayToHexString(cipherText[i]));
			Console.WriteLine("---------------------------------\n");
			
			Console.WriteLine("Init and decode");
			cipher.Init(false, null);
			byte[][] decode = new byte[bindMe.Length][];
			i=0;
			foreach(byte[] msg in cipherText)
			{
				decode[i] = cipher.ProcessBlock(msg, 0, msg.Length);
				i++;
			}
			
			Console.WriteLine("Does it work?:");
			for(i=0; i<bindMe.Length; i++)
				Console.WriteLine("{0}: {1}", bindMe[i]==System.Text.ASCIIEncoding.ASCII.GetString(decode[i])?"Y":"N", System.Text.ASCIIEncoding.ASCII.GetString(decode[i]));
			Console.WriteLine("---------------------------------\n");
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
