// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using System.Xml;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.TPM.Context;
using Iaik.Utils.Hash;

namespace Iaik.Tc.TPM.Configuration.ClientConfiguration
{


	public static class XMLConfiguration
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
					if(conn == null)
						throw new Exception(string.Format("Could not establish connection off type {0}", connType));
					conn.Connect();
					ClientContext ctx = EndpointContext.CreateClientEndpointContext(conn);
					string auth = node.SelectSingleNode("Authentication").Attributes.GetNamedItem("Type").Value;
					ctx.AuthClient.SelectAuthentication(auth);
					if(ctx.AuthClient.Authenticate().Succeeded == false)
						throw new Exception(string.Format("Could not authenticate via {0}", auth));
					foreach(XmlNode dev in node.SelectNodes("TPM"))
					{
						string device = dev.Attributes.GetNamedItem("device").Value;
						if(sessions.ContainsKey(device))
							throw new Exception(string.Format("TPMSession {0} already established", device));
						TPMSession tpm = ctx.TPMClient.SelectTPMDevice(device);
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

