// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
using System;
using System.Collections.Generic;
using Iaik.Tc.TPM.Context;
using Iaik.Utils.Hash;
using Iaik.Utils;
using Iaik.Tc.TPM.Configuration.ClientConfiguration;

namespace Admin
{
	class MainClass
	{
		const string base_path = "/home/andi/Repos/IAIK_ITProject/TpmCSStack-impl/";
		
		public static void Main (string[] args)
		{
			// Establish Connections
			IDictionary<string, TPMSession> sessions =
				XMLConfiguration.EstablischConnection(base_path + "Examples/ClientConfigXml/UnixSocketDeviceLin.xml"); 
			
			// Create one keystore per opened session
			//foreach (TPMSession tpmSes in sessions.Values)
			//	tpmSes.Keystore = new InMemoryKeystore();
			
			TPMSession sessionToUse = sessions["local1"];
			
		//	sessionToUse.SetRequestSecretCallback(RequestSecret);
			
			ProtectedPasswordStorage pws = new ProtectedPasswordStorage();
			pws.WellKnown();
			
			sessionToUse.AdministrationClient.TakeOwnership(ConsoleUtils.ReadPassword("Owner Password: "), pws);
			
			Console.WriteLine("PCRS = " + sessionToUse.CapabilityClient.GetPCRCount());
			
		}
		
	/*	static ProtectedPasswordStorage RequestSecret(HMACKeyInfo keyInfo)
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
			
	*/		                                      
			                                     
	}
}
