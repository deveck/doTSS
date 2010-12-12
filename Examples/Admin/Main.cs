/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


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
		const string base_path = "../../../";
		
		public static void Main (string[] args)
		{
			// Establish Connections
			IDictionary<string, TPMSession> sessions =
				XMLConfiguration.EstablischConnection(base_path + "ClientConfigXml/UnixSocketDeviceLin.xml"); 
			
			// Create one keystore per opened session
			//foreach (TPMSession tpmSes in sessions.Values)
			//	tpmSes.Keystore = new InMemoryKeystore();
			
			TPMSession sessionToUse = sessions["local0"];
			
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
