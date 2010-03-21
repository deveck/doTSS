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
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration;
using Iaik.Tc.TPM.Lowlevel;

namespace Iaik.Tc.TPM
{
	public class TpmServerContext : ServiceBase
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
		
		public TpmServerContext ()
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
			
			SetupTpmContexts ();
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
		private void SetupTpmContexts ()
		{
			IConnectionsConfiguration connectionConfig = (IConnectionsConfiguration)ConfigurationManager.GetSection ("connections");
			
			foreach (Iaik.Tc.TPM.Configuration.DotNetConfiguration.TpmDevice device in connectionConfig.TpmDevices)
			{
				try
				{
					TPMProvider provider = TPMProviders.Create (device.TpmType, device.Parameters);
					TPMContext tpmContext = new TPMContext (device.TpmName, provider);
					_tpmContexts.Add (device.TpmName, tpmContext);
					_logger.InfoFormat ("Successfully setup tpm context '{0}' with type '{1}'", device.TpmName, device.TpmType);
				}
				catch (Exception ex)
				{
					_logger.FatalFormat ("Error setting up tpm device '{0}', the device woll not be available", device.TpmName);
				}
				
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
			log4net.Appender.ConsoleAppender appender = new log4net.Appender.ConsoleAppender();
			appender.Name = "ConsoleAppender";
			appender.Layout = new log4net.Layout.PatternLayout("[%date{dd.MM.yyyy HH:mm:ss,fff}]-%-5level-[%type]: %message%newline");
			log4net.Config.BasicConfigurator.Configure(appender);
			
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
