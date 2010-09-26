
using System;
using Iaik.Utils.Hash;
using System.Xml;
using Iaik.Tc.TPM.Context;
using System.Collections.Generic;
using Iaik.Connection.ClientConnections;

namespace Iaik.Tc.TPM
{


	public static class Utils
	{

		public static ProtectedPasswordStorage ReadPassword (string hintText, TPMConsole console, bool retypePw)
		{
			console.Out.Write (hintText);
			
			ConsoleKeyInfo consoleKeyInfo;
			ProtectedPasswordStorage[] pws;
			
			if(retypePw)
				pws = new ProtectedPasswordStorage[] { new ProtectedPasswordStorage (), new ProtectedPasswordStorage () };
			else
				pws = new ProtectedPasswordStorage[] { new ProtectedPasswordStorage() };
			
			for (int i = 0; i < pws.Length; i++)
			{
				ProtectedPasswordStorage pw = pws[i];

				if (i == 1)
					console.Out.Write ("Retype password:");
				
				while (true)
				{
					consoleKeyInfo = console.ReadKeyWithoutEcho ();
					if (consoleKeyInfo.Key == ConsoleKey.Enter)
					{
						console.Out.WriteLine ();
						break;
					}
					else if (consoleKeyInfo.Key == ConsoleKey.Escape)
					{
						console.Out.WriteLine ();
						return null;
					}
					else
						pw.AppendPasswordChar (consoleKeyInfo.KeyChar);
				}
			}

            if (retypePw == false || pws[0].EqualPassword(pws[1]))
            {
                pws[0].Hash();
                return pws[0];
            }
            else
            {
                console.Out.WriteLine("Error: Passwords do not match!");
                return null;
            }
		}
		
		public static class XmlConfiguration
		{
			public static IDictionary<string, TPMSession> EstablischConnection(string filename){
				IDictionary<string, TPMSession> sessions = new Dictionary<string, TPMSession>();
				
				XmlDocument xdoc = new XmlDocument();
				xdoc.Load(filename);
				
				XmlNode rootNode = xdoc.SelectSingleNode("TPMClientConfiguration");
				
				XmlNodeList clientNodes = rootNode.SelectNodes("Context");
				
				foreach(XmlNode node in clientNodes)
				{
					string connType = node.SelectSingleNode("Connection").Attributes.GetNamedItem("Type").Value;
					IDictionary<string, string> connSpecAttr = GetAttributesDictionary(node.SelectSingleNode("Connection").SelectSingleNode(connType));
					ConnectionBuilderSettings settings = new ConnectionBuilderSettings(RequestSecret);
					FrontEndConnection conn = ConnectionFactory.CreateFrontEndConnection(connType, settings, connSpecAttr);
					conn.Connect();
					ClientContext ctx = EndpointContext.CreateClientEndpointContext(conn);
					string auth = node.SelectSingleNode("Authentication").Attributes.GetNamedItem("Type").Value;
					ctx.AuthClient.SelectAuthentication(auth);
					ctx.AuthClient.Authenticate();
					foreach(XmlNode dev in node.SelectNodes("TPM"))
					{
						TPMSession tpm = ctx.TPMClient.SelectTPMDevice(dev.Attributes.GetNamedItem("device").Value);
						sessions.Add(dev.Attributes.GetNamedItem("alias").Value, tpm);
					}
				}
				
				return sessions;
			}
			
			private static IDictionary<string, string> GetAttributesDictionary(XmlNode node)
			{
				Dictionary<string, string> dict = new Dictionary<string, string>();
				
				foreach(XmlNode attr in node.Attributes)
					dict.Add(attr.Name, attr.Value);
				
				return dict;
			}
			
			private static ProtectedPasswordStorage RequestSecret(string hintText)
			{
							return null;
			}
		}
		
	}
}
