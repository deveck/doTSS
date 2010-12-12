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

namespace SignVerify
{
	class MainClass
	{
		const string base_path = "../../../";
		
		public static void Main (string[] args)
		{
			string quoteMe = "Hallo IAIK!";
			byte[] quoteMeBytes = System.Text.Encoding.ASCII.GetBytes(quoteMe);
			
			// Establish Connections
			IDictionary<string, TPMSession> sessions =
				XMLConfiguration.EstablischConnection(base_path + "ClientConfigXml/UnixSocketDeviceLin.xml"); 
			
			// Create one keystore per opened session
			foreach (TPMSession tpmSes in sessions.Values)
				tpmSes.Keystore = new InMemoryKeystore();
			
			TPMSession sessionToUse = sessions["local0"];
			
			sessionToUse.SetRequestSecretCallback(RequestSecret);
			
			ClientKeyHandle myFirstSignKey = 
				sessionToUse.KeyClient.GetSrkKeyHandle().CreateKey("my_first_sign_key", TPMKeyUsage.TPM_KEY_SIGNING);

			ISigner signer = myFirstSignKey.CreateSigner();
			
			signer.Init(true, null);
			signer.BlockUpdate(quoteMeBytes, 0, quoteMeBytes.Length);
			
			byte[] quote = signer.GenerateSignature();
			
			Console.WriteLine("Sign of \"Hallo IAIK\" is:\n" + ByteHelper.ByteArrayToHexString(quote));
			
			Console.WriteLine();
			Console.WriteLine("Now we would verify this sign.");
			
			signer.Reset();
			signer.Init(false, null);
			signer.BlockUpdate(quoteMeBytes, 0, quoteMeBytes.Length);
			
			if(signer.VerifySignature(quote) == true)
			{
				Console.WriteLine("Sign is OK!");
			}
			else
			{
				Console.WriteLine("UUUUPPPPSSS something went wrong!");
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
