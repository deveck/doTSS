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
