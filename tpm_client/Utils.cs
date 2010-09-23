
using System;
using Iaik.Utils.Hash;
using System.Xml;
using Iaik.Tc.TPM.Context;
using System.Collections.Generic;

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
			public static ClientContext EstablischConnection(string filename){
				XmlDocument xdoc = new XmlDocument();
				xdoc.Load(filename);
				
				XmlNode rootNode = xdoc.SelectSingleNode("TPMClientConfiguration");
				
				string tpmDevice = rootNode.SelectSingleNode("TPM").Attributes.GetNamedItem("Name").Value;
				
				XmlNode node = rootNode.SelectSingleNode("Connection");
				string connType = node.Attributes.GetNamedItem("Type").Value;
				
				IDictionary<string, string> connSpecAttr = GetAttributesDictionary(node.SelectSingleNode(connType));
				
				node = rootNode.SelectSingleNode("Authentication");
				string authType = node.Attributes.GetNamedItem("Type").Value;
				
				IDictionary<string, string> authSpecAttr = GetAttributesDictionary(node.SelectSingleNode(authType));
				
				Console.WriteLine(tpmDevice);
				Console.WriteLine(connType);
				foreach(KeyValuePair<string, String> p in connSpecAttr)
					Console.WriteLine(p.Key +"="+ p.Value);
				Console.WriteLine(authType);
				foreach(KeyValuePair<string, String> p in authSpecAttr)
					Console.WriteLine(p.Key +"="+ p.Value);
				
				return null;
			}
			
			private static IDictionary<string, string> GetAttributesDictionary(XmlNode node)
			{
				Dictionary<string, string> dict = new Dictionary<string, string>();
				
				foreach(XmlNode attr in node.Attributes)
					dict.Add(attr.Name, attr.Value);
				
				return dict;
			}
		}
		
	}
}
