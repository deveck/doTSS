// 
// 
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration
{

	[ConfigurationCollection(typeof(AuthenticationElement),
	                         AddItemName="addAuth",
	                         RemoveItemName="removeAuth",
	                         ClearItemsName="clearAuth")]
	public class AuthenticationMethodCollection : ConfigurationElementCollection
	{

		protected override ConfigurationElement CreateNewElement ()
		{
			return new AuthenticationElement();
		}

		protected override object GetElementKey (ConfigurationElement element)
		{
			return (element as AuthenticationElement).AuthIdentifier;
		}

		protected override string ElementName 
		{
			get{ return "addAuth"; }
		}

		
	}
}
