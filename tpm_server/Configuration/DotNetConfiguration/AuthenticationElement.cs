// 
// 
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration
{


	public class AuthenticationElement : ConfigurationElement
	{
		private const string PROP_TYPE = "type";
		
		/// <summary>
		/// Returns 
		/// </summary>
		[ConfigurationProperty(PROP_TYPE, IsRequired=true)]
		public string AuthType
		{
			get{ return (string)base[PROP_TYPE]; }
		}
	}
}
