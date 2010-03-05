// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using Iaik.Tc.Tpm.Configuration;
using Iaik.Connection.Configuration;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration
{


	/// <summary>
	/// .net configuration implementation of the IListenerConfiguration interface
	/// </summary>
	[ConfigurationCollection(typeof(Parameter))]	                         
	public class Listener : ConfigurationElementCollection, IListenerConfiguration
	{
		private const string PROP_LISTENER_TYPE = "type";
		
		#region IListenerConfiguration implementation
		/// <summary>
		/// Returns the type of the listener
		/// </summary>
		[ConfigurationProperty(PROP_LISTENER_TYPE, IsRequired=true)]
		public string ListenerType
		{
			get{ return (string)base[PROP_LISTENER_TYPE];}
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
		public IEnumerable<KeyValuePair<string, string>> Parameters 
		{
			get 
			{
				List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
				
				foreach(Parameter param in this)
					parameters.Add(new KeyValuePair<string, string>(param.Name, param.Value));
				
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
