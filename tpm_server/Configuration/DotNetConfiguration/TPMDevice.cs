// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using Iaik.Tc.TPM.Configuration;
using Iaik.Connection.Configuration;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
{


	/// <summary>
	/// Provides the .net configuration of the tpm device configuration
	/// </summary>
	[ConfigurationCollection(typeof(Parameter))]	                         
	public class TPMDevice : ConfigurationElementCollection, ITPMDeviceConfiguration
	{
		private const string PROP_DEVICE_TYPE = "type";
		private const string PROP_DEVICE_NAME = "name";
		
		#region IListenerConfiguration implementation
		/// <summary>
		/// Returns the type of the tpm device
		/// </summary>
		[ConfigurationProperty(PROP_DEVICE_TYPE, IsRequired=true)]
		public string TPMType
		{
			get { return (string)base[PROP_DEVICE_TYPE];}
		}	
		
		/// <summary>
		/// Returns the name of the tpm device
		/// </summary>
		[ConfigurationProperty(PROP_DEVICE_NAME, IsRequired = true)]
		public string TPMName {
			get { return (string)base[PROP_DEVICE_NAME]; }
		}
		
		/// <summary>
		/// Looks for a named parameter and returns its value or null
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string FindParameter (string name)
		{
			foreach(Parameter param in this)
			{
				if(param.Name.Equals(name))
					return param.Value;
			}
			
			return null;
		}
		
		
		/// <summary>
		/// Returns all defined parameters
		/// </summary>
		public IDictionary<string, string> Parameters 
		{
			get 
			{
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				
				foreach(Parameter param in this)
					parameters.Add(param.Name, param.Value);
				
				return parameters;
			}
		}
		
	
		#endregion
		
		protected override ConfigurationElement CreateNewElement ()
		{
			return new Parameter();
		}
		
		protected override object GetElementKey (ConfigurationElement element)
		{
			return (element as Parameter).Name;
		}
		
	}
}
