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
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Configuration.ClientConfiguration;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.PCRData;
using Org.BouncyCastle.Crypto;
using Iaik.Utils;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using System.Collections.Generic;
using Iaik.Tc.TPM.Keystore.Backends;

namespace Quote
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
			
			ClientKeyHandle myFirstQuoteKey = 
				sessionToUse.KeyClient.GetSrkKeyHandle().CreateKey("my_first_quote_key", TPMKeyUsage.TPM_KEY_SIGNING);
			
			sessionToUse.IntegrityClient.Extend(0, new byte[]{0,1,2,3,4,5,6,7,8,9,9,8,7,6,5,4,3,2,1,0});
			sessionToUse.IntegrityClient.Extend(1, new byte[]{0,1,2,3,4,5,6,7,8,9,9,8,7,6,5,4,3,2,1,0});
			sessionToUse.IntegrityClient.Extend(2, new byte[]{0,1,2,3,4,5,6,7,8,9,9,8,7,6,5,4,3,2,1,0});
			
			TPMPCRSelection pcrselect = sessionToUse.CreateEmptyPCRSelection();
			
			pcrselect.PcrSelection[0] = true;
			pcrselect.PcrSelection[1] = true;
			pcrselect.PcrSelection[2] = true;
			
			ISigner signer = myFirstQuoteKey.CreateQuoter(pcrselect);
			
			signer.Init(true, null);
			signer.BlockUpdate(quoteMeBytes, 0, quoteMeBytes.Length);
			
			byte[] quote = signer.GenerateSignature();
			
			Console.WriteLine("Quote of \"Hallo IAIK\" is:\n" + ByteHelper.ByteArrayToHexString(quote));
			
			Console.WriteLine();
			Console.WriteLine("Now we would verify this quote.");
			
			signer.Reset();
			signer.Init(false, null);
			signer.BlockUpdate(quoteMeBytes, 0, quoteMeBytes.Length);
			
			if(signer.VerifySignature(quote) == true)
			{
				Console.WriteLine("Quote is OK!");
			}
			else
			{
				Console.WriteLine("UUUUPPPPSSS something went wrong!");
			}
			
			
			Console.WriteLine("Extending PCRs, Quote should fail now!");
			
			sessionToUse.IntegrityClient.Extend(0, new byte[]{0,1,2,3,4,5,6,7,8,9,9,8,7,6,5,4,3,2,1,0});
			sessionToUse.IntegrityClient.Extend(1, new byte[]{0,1,2,3,4,5,6,7,8,9,9,8,7,6,5,4,3,2,1,0});
			sessionToUse.IntegrityClient.Extend(2, new byte[]{0,1,2,3,4,5,6,7,8,9,9,8,7,6,5,4,3,2,1,0});
			
			if(signer.VerifySignature(quote) == true)
			{
				Console.WriteLine("UUUUPPPPSSS something went wrong!");	
			}
			else
			{
				Console.WriteLine("Quote is NOT OK, that's the way it should be.");
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
