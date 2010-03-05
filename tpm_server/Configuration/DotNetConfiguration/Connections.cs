// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz>

using System;
using System.Configuration;
using System.Collections.Generic;
using Iaik.Tc.Tpm.Configuration;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration
{

	/// <summary>
	/// Parses the connections tag from the configuration file
	/// </summary>
	public class Connections : ConfigurationSection, IConnectionsConfiguration
	{
		/// <summary>
		/// Listeners tag
		/// </summary>
		private const string PROP_LISTENERS = "listeners";
		
		/// <summary>
		/// Returns the collection of configured listeners
		/// </summary>
		[ConfigurationProperty(PROP_LISTENERS)]
		public ListenersCollection Listeners
		{
			get{ return (ListenersCollection)base[PROP_LISTENERS]; }
		}

		
		#region IConnectionsConfiguration implementation
		
		/// <summary>
		/// Returns all defined Listeners
		/// </summary>
		IEnumerable<IListenerConfiguration> IConnectionsConfiguration.Listeners 
		{
			get 
			{
				List<IListenerConfiguration> listenerConfigurations = new List<IListenerConfiguration>();
				
				foreach(IListenerConfiguration listenerConfiguration in Listeners)
					listenerConfigurations.Add(listenerConfiguration);
				
				return listenerConfigurations;
			}
		}
		
		#endregion
	}
}
