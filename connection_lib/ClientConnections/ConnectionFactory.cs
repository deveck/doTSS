// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.CommonFactories;

namespace Iaik.Connection.ClientConnections
{


	public static class ConnectionFactory
	{
		public static FrontEndConnection CreateFrontEndConnection(
		           string identifier,
		           ConnectionBuilderSettings settings, 
		           params object[] parameters)
		{
			Type t = GenericClassIdentifierFactory.FindTypeForIdentifier<FrontEndConnection>(identifier);
			
			if( t == null)
				return null;
			
			FrontEndConnectionAttribute attr = (FrontEndConnectionAttribute)t.GetCustomAttributes(typeof(FrontEndConnectionAttribute), false)[0];
			
			if(attr.ConnectionBuilder != null)
			{
				IConnectionBuilder connectionBuilder = (IConnectionBuilder)
					GenericClassIdentifierFactory.CreateInstance(attr.ConnectionBuilder, parameters);
				
				connectionBuilder.Settings = settings;
				return connectionBuilder.SetupConnection();
			}
			else
				return GenericClassIdentifierFactory.CreateFromClassIdentifierOrType<FrontEndConnection>(identifier, parameters);
			
		}
	}
}
