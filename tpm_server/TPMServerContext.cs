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
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.ServiceProcess;
using log4net;
using Iaik.Connection.Configuration;
using Iaik.Utils.CommonFactories;
using Iaik.Connection;
using System.Collections.Generic;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.TPM.Context;
using System.Configuration;
using Iaik.Tc.TPM.Configuration;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration;
using Iaik.Tc.TPM.Lowlevel;

using Iaik.Tc.TPM.Subsystems.TPMSubsystem;
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles;
using System.Reflection;
using System.Xml;

namespace Iaik.Tc.TPM
{
	public class TPMServerContext : ServiceBase
	{
		/// <summary>
		/// The ServerContextLogger
		/// </summary>
		private ILog _logger = null;

		/// <summary>
		/// Contains all running listeners
		/// </summary>
		private List<IFrontEndServerListener> _listeners = new List<IFrontEndServerListener>();
		
		/// <summary>
		/// Contains all active ServerContexts (ClientConnections)
		/// </summary>
		private List<ServerContext> _activeContexts = new List<ServerContext>();
		
		/// <summary>
		/// Contains all active tpm contexts
		/// </summary>
		private Dictionary<string, TPMContext> _tpmContexts = new Dictionary<string, TPMContext>();
		
		/// <summary>
		/// Contains all defined Users/Groups with their associated permissions
		/// </summary>
		private AccessControlList _accessControlList = null;
		
		public TPMServerContext ()
		{
		}
		
		public void Start()
		{
			OnStart(null);
		}
		
		protected override void OnStart (string[] args)
		{
			base.OnStart (args);
			
			SetupLogging ();
			
			_accessControlList = new DotNetCfgAccessControlList ();
			
			SetupTPMContexts ();
			StartConnections ();
			
		}
		
		protected override void OnStop ()
		{
			base.OnStop ();
		}
		
		/// <summary>
		/// Reads the configured tpm devices from the configuration and
		/// sets up the corresponding tpm contexts
		/// </summary>
		private void SetupTPMContexts ()
		{
			IConnectionsConfiguration connectionConfig = (IConnectionsConfiguration)ConfigurationManager.GetSection ("connections");
			
			foreach (Iaik.Tc.TPM.Configuration.DotNetConfiguration.TPMDevice device in connectionConfig.TpmDevices)
			{
				try
				{
					_logger.InfoFormat ("Setting up tpm context '{0}'", device.TPMName);
					TPMWrapper tpmDevice = new TPMWrapper ();
					tpmDevice.Init (device.TPMType, device.Parameters);
					TPMContext tpmContext = new TPMContext (device.TPMName, tpmDevice);
					_tpmContexts.Add (device.TPMName, tpmContext);
					
					_logger.InfoFormat ("Flushing device '{0}'", device.TPMName);
					foreach (TPMResourceType resourceType in new TPMResourceType[] {
						TPMResourceType.TPM_RT_AUTH, TPMResourceType.TPM_RT_KEY})
					{
						Parameters listLoadedHandlesParameters = new Parameters ();
						listLoadedHandlesParameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_HANDLE);
						listLoadedHandlesParameters.AddPrimitiveType ("handle_type", resourceType);
						TPMCommandRequest listLoadedHandlesRequest = new TPMCommandRequest (TPMCommandNames.TPM_CMD_GetCapability, 
							listLoadedHandlesParameters);
						TPMCommandResponse response = tpmDevice.Process (listLoadedHandlesRequest);
						
						if (response.Status == false)
							throw new Exception ("An unknown tpm exception while flushing occured");
						
						foreach (uint handle in response.Parameters.GetValueOf<HandleList> ("handles"))
						{
							Parameters flushParameters = new Parameters ();
							flushParameters.AddValue ("handle", HandleFactory.Create (resourceType, handle));
							TPMCommandRequest flushRequest = new TPMCommandRequest (TPMCommandNames.TPM_CMD_FlushSpecific, flushParameters);
							TPMCommandResponse flushResponse = tpmDevice.Process (flushRequest);
							
							if (flushResponse.Status == false)
								throw new Exception ("Something went wrong while flushing");
							
						}
					}
					
					_logger.InfoFormat ("Successfully setup tpm context '{0}' with type '{1}'", device.TPMName, device.TPMType);
				}
				catch (Exception ex)
				{
					_logger.FatalFormat ("Error setting up tpm device '{0}', the device will not be available ({1})", device.TPMName, ex);
				}
				
				///Set the Assembly search order for incoming Parameters so that core classes are always at first
				Parameters.AssemblySearchOrder = new Assembly[]{
					typeof(TPMWrapper).Assembly, //lib core
					typeof(ITypedParameter).Assembly};	//lib common
				
			}
		}
		
		/// <summary>
		/// Starts the configured connection listeners
		/// </summary>
		private void StartConnections()
		{
			IConnectionsConfiguration connectionConfig = (IConnectionsConfiguration)ConfigurationManager.GetSection("connections");
			
			foreach(IListenerConfiguration listenerConfig in connectionConfig.Listeners)
			{
				_logger.DebugFormat("Listener configuration: {0}", listenerConfig.ListenerType);
				_logger.DebugFormat("\tWith parameters:");
				
				foreach(KeyValuePair<string, string> parameter in listenerConfig.Parameters)
					_logger.DebugFormat("\tname={0}; value={1}", parameter.Key, parameter.Value);
				
				IFrontEndServerListener listener = GenericClassIdentifierFactory.CreateFromClassIdentifierOrType<IFrontEndServerListener>(
					listenerConfig.ListenerType, listenerConfig);
				
				if(listener == null)
				{
					_logger.FatalFormat("Cannot start listener '{0}', listener not found!", listenerConfig.ListenerType);
					throw new ArgumentException(string.Format("Cannot start listener '{0}', listener not found!", listenerConfig.ListenerType));
				}
				
				
				_logger.InfoFormat("Listener '{0}' started succesfull (type={1})", listenerConfig.ListenerType, listener.GetType());
				_listeners.Add(listener);
				listener.ClientConnected += HandleListenerClientConnected;
				listener.Listen();
			}
		}		
		
		/// <summary>
		/// Initializes the logger
		/// </summary>
		private void SetupLogging()
		{
			if(ConfigurationManager.GetSection("log4net") == null)			
			{
				log4net.Appender.ConsoleAppender appender = new log4net.Appender.ConsoleAppender();	
				appender.Name = "ConsoleAppender";
				appender.Layout = new log4net.Layout.PatternLayout("[%date{dd.MM.yyyy HH:mm:ss,fff}]-%-5level-%t-[%c]: %message%newline");
				log4net.Config.BasicConfigurator.Configure(appender);
			}
			else
			{
				log4net.Config.XmlConfigurator.Configure((XmlElement)ConfigurationManager.GetSection("log4net"));
			}
			
			_logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		}
		
		/// <summary>
		/// A new client connected to a listener
		/// </summary>
		/// <param name="connection">The connection to the client</param>
		private void HandleListenerClientConnected (FrontEndConnection connection)
		{
			_logger.InfoFormat ("Client {0} connected", connection);
			ServerContext ctx;
			lock (_activeContexts)
			{
				ctx = EndpointContext.CreateServerEndpointContext (connection, 
					(IConnectionsConfiguration)ConfigurationManager.GetSection("connections"),
					_accessControlList, _tpmContexts);			
				_activeContexts.Add(ctx);
			}
			connection.Disconnected += delegate 
			{
				lock(_activeContexts)
				{
					_logger.InfoFormat("Client {0} disconnected", connection);
					_activeContexts.Remove(ctx);					
				}
			};
		}

	}
}
