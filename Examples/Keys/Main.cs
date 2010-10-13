// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
using System;
using Iaik.Tc.TPM.Context;
using System.Collections;
using Iaik.Tc.TPM.Configuration.ClientConfiguration;
using System.Collections.Generic;
using Iaik.Tc.TPM.Keystore.Backends;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Utils;

namespace Keys
{
	class MainClass
	{
		const string base_path = "../../../";
		
		public static void Main (string[] args)
		{
			// Establish Connections
			IDictionary<string, TPMSession> sessions =
				XMLConfiguration.EstablischConnection(base_path + "ClientConfigXml/UnixSocketDeviceLin.xml"); 
			
			// Create one keystore per opened session
 			foreach (TPMSession tpmSes in sessions.Values)
				tpmSes.Keystore = new InMemoryKeystore();

			
			TPMSession sessionToUse = sessions["local0"];
			sessionToUse.SetRequestSecretCallback(RequestSecret);
			
			
			Console.WriteLine("Creating key 'my_first_storage_key'");			
			ClientKeyHandle myFirstStorageKey = 
				sessionToUse.KeyClient.GetSrkKeyHandle().CreateKey("my_first_storage_key", TPMKeyUsage.TPM_KEY_STORAGE);
			Console.WriteLine("Created key 'my_first_storage_key' with public key: {0}", myFirstStorageKey.PublicKey.PublicKey);
			Console.WriteLine("------------------------\n\n");
			
			Console.WriteLine("Creating key 'my_second_storage_key'");
			ClientKeyHandle mySecondStorageKey = myFirstStorageKey.CreateKey("my_second_storage_key", TPMKeyUsage.TPM_KEY_STORAGE);
			Console.WriteLine("Created key 'my_second_storage_key' with public key: {0}", mySecondStorageKey.PublicKey.PublicKey);
			Console.WriteLine("------------------------\n\n");
			
			
			Console.WriteLine("Creating key 'binding_key'");
			ClientKeyHandle bindingKey = mySecondStorageKey.CreateKey("binding_key", TPMKeyUsage.TPM_KEY_BIND);
			Console.WriteLine("Created key 'binding_key' with public key: {0}", bindingKey.PublicKey.PublicKey);
			Console.WriteLine("------------------------\n\n");
			
			Console.WriteLine("Keystore now contains {0} keys", sessionToUse.Keystore.EnumerateFriendlyNames().Length);
			
			foreach(String keyFriendlyName in sessionToUse.Keystore.EnumerateFriendlyNames())
			{
				KeyValuePair<string,string>? parent = sessionToUse.Keystore.FindParentKeyByFriendlyName(keyFriendlyName);
				Console.WriteLine("Key: '{0}' with parent '{1}'", keyFriendlyName, 
				     parent == null?"srk":parent.Value.Key);
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
			
			return ConsoleUtils.ReadPassword(String.Format("Please enter Passwd for key {0}: ",
			                                               keyInfo.Parameters.GetValueOf<string>("identifier")));
			
		}
			
			                                      
			                                     
	}
}
