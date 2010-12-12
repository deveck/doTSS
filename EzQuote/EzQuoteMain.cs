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
using Iaik.Connection.ClientConnections;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Keystore;
using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto;
using Iaik.Tc.TPM.Library.Common.PCRData;

namespace EzQuote
{
	public class EzQuoteMain : IDisposable
	{		
		/// <summary>
		/// Client context for interaction with the DoTSS server side.
		/// </summary>
		private ClientContext ctx_;
		
		public EzQuoteMain ()
		{
			// Establish the uplink connection (using local Unix domain socket)
			FrontEndConnection local = new UnixSocketConnection("/tmp/dotss.socket");			
			local.Connect();
			
			// Instantiate the client context
			ctx_ = ClientContext.CreateClientEndpointContext(local);
			
			// Authenticate the local user
			ctx_.AuthClient.SelectAuthentication("unix_auth");
			ctx_.AuthClient.Authenticate();
			
			ctx_.DebugClient.PrintOnServerConsole("ready to rumble");
			
			
		}
		
		public static void Main(String[] args)
		{
			using (EzQuoteMain main = new EzQuoteMain()) 
			{
				/*foreach (String tpm_name in main.ctx_.TPMClient.TPMDevices) 
				{
					Console.WriteLine("TPM DEVICE {0}", tpm_name);
					
				}*/
				
				TPMSession tpm0 = main.ctx_.TPMClient.SelectTPMDevice("ibm0");
				
				IDictionary<string, string> opts =  new Dictionary<string,string>();
				opts.Add("file", "/tmp/mystore");
				tpm0.Keystore = TPMKeystoreProviders.Create("SQLiteKeystore", opts);			
				tpm0.SetRequestSecretCallback(mycallback);
				
				/*
				ProtectedPasswordStorage pws = new ProtectedPasswordStorage();
				pws.AppendPasswordChar('i');
				pws.AppendPasswordChar('a');
				pws.AppendPasswordChar('i');
				pws.AppendPasswordChar('k');
				
				tpm0.AdministrationClient.TakeOwnership(pws, pws);
				*/
				
				ClientKeyHandle kh_srk = tpm0.KeyClient.GetSrkKeyHandle();
				ClientKeyHandle kh_sig1 = kh_srk.CreateKey("sigkey5" + tpm0.CreateRNG().Next(), 2048, TPMKeyUsage.TPM_KEY_SIGNING,
				                                           TPMKeyFlags.None);
				
				TPMPCRSelection pcrs = tpm0.CreateEmptyPCRSelection();
				pcrs.PcrSelection.SetBit(0, true);
				pcrs.PcrSelection.SetBit(1, true);
				pcrs.PcrSelection.SetBit(16, true);
				
				foreach(int pcr in pcrs.SelectedPCRs)
				{
					Console.Write(" PCR {0:D2}: 0x");
					foreach (byte b in tpm0.IntegrityClient.PCRValue((uint)pcr)) 
					{
						Console.Write("{0:X2}", b);
					}
					Console.WriteLine();
				}
				
				ISigner signer =  kh_sig1.CreateQuoter(pcrs);
				signer.Init(true, null);
				signer.Update((byte)'i');
				signer.Update((byte)'a');
				signer.Update((byte)'i');
				signer.Update((byte)'k');
				byte[] signature = signer.GenerateSignature();
				
				Console.Write("QUOTE: ");
				foreach (byte b in signature) {
					Console.Write(" {0:X2}", b);
				}
				Console.WriteLine();
				
				ISigner verifier = kh_sig1.CreateQuoter(pcrs);
				verifier.Init(false, null);
				verifier.Update((byte)'i');
				verifier.Update((byte)'a');
				verifier.Update((byte)'i');
				verifier.Update((byte)'k');
				if (verifier.VerifySignature(signature)) {
					Console.WriteLine("JO IT WORKED");
				} else {
					Console.WriteLine("NA IT FAILED");
				}
			}
		}
				                              
		public static ProtectedPasswordStorage mycallback(HMACKeyInfo keyInfo)
		{
			// We use the empty string as password ...
			ProtectedPasswordStorage pws = new ProtectedPasswordStorage();
			pws.AppendPasswordChar('i');
			pws.AppendPasswordChar('a');
			pws.AppendPasswordChar('i');
			pws.AppendPasswordChar('k');
			return pws;
		}                            
		
		#region IDisposable implementation
		public void Dispose ()
		{
			try 
			{
				ctx_.Connection.Close();
			} 
			finally 
			{
				ctx_ = null;
			}
		}
		
		#endregion	
	}
}