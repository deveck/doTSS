// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
{
	
	public class Parameter : ConfigurationElement
	{
		private const string PROP_NAME = "name";
		private const string PROP_VALUE = "value";
		
		
		/// <summary>
		/// Returns the name of the parameter
		/// </summary>
		[ConfigurationProperty(PROP_NAME, IsRequired=true, IsKey=true)]
		public string Name
		{
			get{ return (string)base[PROP_NAME];}
		}
		
		/// <summary>
		/// Returns the value of the parameter
		/// </summary>
		[ConfigurationProperty(PROP_VALUE, IsRequired=true)]
		public string Value
		{
			get{ return (string)base[PROP_VALUE];}
		}

	}
}
