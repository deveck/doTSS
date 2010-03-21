// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz>

using System;
using System.Configuration;
using System.Collections.Generic;
using Iaik.Tc.TPM.Configuration;
using Iaik.Connection.Configuration;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
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
		/// Authentication methods tag
		/// </summary>
		private const string PROP_AUTHENTICATIONMETHODS = "authenticationMethods";
		
		/// <summary>
		/// Authentication methods tag
		/// </summary>
		private const string PROP_TPMDEVICES = "tpmDevices";
		
		/// <summary>
		/// Returns the collection of configured listeners
		/// </summary>
		[ConfigurationProperty(PROP_LISTENERS)]
		public ListenersCollection Listeners
		{
			get{ return (ListenersCollection)base[PROP_LISTENERS]; }
		}

		[ConfigurationProperty(PROP_AUTHENTICATIONMETHODS)]
		public AuthenticationMethodCollection AuthenticationMethods
		{
			get { return (AuthenticationMethodCollection)base[PROP_AUTHENTICATIONMETHODS]; }
		}
		
		[ConfigurationProperty(PROP_TPMDEVICES)]
		public TPMDeviceCollection TpmDevices {
			get { return (TPMDeviceCollection)base[PROP_TPMDEVICES]; }
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
		
		IEnumerable<IAuthenticationMethod> IConnectionsConfiguration.AuthenticationMethods
		{
			get
			{
				List<IAuthenticationMethod> authenticationMethods = new List<IAuthenticationMethod> ();
				
				foreach (IAuthenticationMethod authMethod in AuthenticationMethods)
					authenticationMethods.Add (authMethod);
				
				return authenticationMethods;
			}
		}
		
		IEnumerable<ITPMDeviceConfiguration> IConnectionsConfiguration.TpmDevices {
			get {
				List<ITPMDeviceConfiguration> tpmDevices = new List<ITPMDeviceConfiguration> ();
				
				foreach (ITPMDeviceConfiguration tpmDevice in TpmDevices)
					tpmDevices.Add (tpmDevice);
				
				return tpmDevices;
			}
		}
		
		#endregion
	}
}
