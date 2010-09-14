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
				
				/*Random rng = tpm0.CreateRNG();
				for (int n = 0; n < 100; ++n) {
					Console.WriteLine("random: {0}",	rng.Next());
				}*/
				
				Console.WriteLine(tpm0.EndpointCtx.PacketTransmitter.RequestPacketReceived);

				
				ClientKeyHandle khsrk = tpm0.KeyClient.GetSrkKeyHandle();			
				Console.WriteLine("SRK handle {0}", khsrk);
				
				main.ctx_.DebugClient.PrintOnServerConsole("ready to rumble 2");
				main.ctx_.DebugClient.PrintOnServerConsole("ready to rumble 3 ");
			
				
				ClientKeyHandle khbind = khsrk.CreateKey("mykey", TPMKeyUsage.TPM_KEY_BIND);
				Console.WriteLine("Bind handle {0}", khsrk);
				main.ctx_.DebugClient.PrintOnServerConsole("ready to rumble 4");
									
				/*Org.BouncyCastle.Crypto.IAsymmetricBlockCipher block = khbind.CreateBindBlockCipher();
				byte[] outs = block.ProcessBlock(new byte[] { 0xca, 0xfe, 0xba, 0xbe}, 0, 4);
				Console.WriteLine(outs);*/
				
				
			}
		}
				                              
		public static ProtectedPasswordStorage mycallback(HMACKeyInfo keyInfo)
		{
			ProtectedPasswordStorage pws = new ProtectedPasswordStorage();
			pws.WellKnown();
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