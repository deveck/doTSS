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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Connection.ClientConnections;
using Iaik.Connection.Packets;
using Iaik.Tc.TPM.Subsystems;
using System.Threading;
using Iaik.Tc.TPM.Configuration;
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Subsystems.TPMSubsystem;
using log4net;

namespace Iaik.Tc.TPM.Context
{

    /// <summary>
    /// Manages one client connection to the server with all its subsystems
    /// </summary>
    public abstract class EndpointContext
    {
		/// <summary>
		/// Contains all registered subsystems.
		/// The subsystems get registered by the finally implementing classes
		/// </summary>
		private Dictionary<string, ISubsystem> _subsystems = new Dictionary<string, ISubsystem>();
		
		
		/// <summary>
		/// Logger
		/// </summary>
		protected ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		/// <summary>
		/// Creates a ServerContext for the specified connection
		/// </summary>
		public static ServerContext CreateServerEndpointContext(FrontEndConnection connection, IConnectionsConfiguration connectionConfig,
			AccessControlList acl, IDictionary<string, TPMContext> tpmContexts)
		{
			PacketTransmitter packetTransmitter = new PacketTransmitter(connection);
			ServerContext ctx = new ServerContext(connection, packetTransmitter, connectionConfig, acl, tpmContexts);
			packetTransmitter.StartTransmitting();
			return ctx;
		}
		
		/// <summary>
		/// Creates a ClientContext for the specified connection
		/// </summary>
		public static ClientContext CreateClientEndpointContext(FrontEndConnection connection)
		{
			PacketTransmitter packetTransmitter = new PacketTransmitter(connection);
			ClientContext ctx = new ClientContext(connection, packetTransmitter);
			packetTransmitter.StartTransmitting();
			return ctx;			
		}
		
		/// <summary>
		/// Raw connection to the client
		/// </summary>
		protected FrontEndConnection _connection;
		
		/// <summary>
		/// Responsible for the transmition of packets
		/// </summary>
		protected PacketTransmitter _packetTransmitter;
		
		/// <summary>
		/// Is set by the base class once the configuration is finished
		/// </summary>
		protected volatile bool _configured = false;
		
		protected AutoResetEvent _configuredEvent = new AutoResetEvent(false);
		
		public FrontEndConnection Connection
		{
			get{ return _connection; }
		}
		
		public PacketTransmitter PacketTransmitter
		{
			get{ return _packetTransmitter;}
		}
		
		public EndpointContext(FrontEndConnection connection, PacketTransmitter packetTransmitter)
		{
			_connection = connection;
			connection.Disconnected += HandleConnectionDisconnected;
			_packetTransmitter = packetTransmitter;
			_packetTransmitter.RequestPacketReceived += OnRequestPacketReceived;
		}

		/// <summary>
		///Is called once the client disconnects 
		/// </summary>
		/// <param name="obj">A <see cref="FrontEndConnection"/></param>
		protected void HandleConnectionDisconnected (FrontEndConnection obj)
		{
			foreach(ISubsystem subsystem in _subsystems.Values)
				subsystem.Dispose();
			_subsystems.Clear();		
		}

		/// <summary>
		/// Is called when a new DataPacket from the remote end point receives
		/// </summary>
		private void OnRequestPacketReceived (DataPacket packet)
		{
			if(_configured == false)
				_configuredEvent.WaitOne();
				
			if(_subsystems.ContainsKey(packet.Subsystem))
			{
				_logger.DebugFormat("Redirecting packet to subsystem '{0}'", packet.Subsystem);
				_subsystems[packet.Subsystem].HandlePacket(packet);
			}
			else
			{
				 _logger.WarnFormat("Subsystem '{0}' has not been found", packet.Subsystem);
			}
		}		

		/// <summary>
		/// Registers the specified subsystem in this context
		/// </summary>
		protected void RegisterSubsystem(ISubsystem subsystem)
		{
			if(_subsystems.ContainsKey(subsystem.SubsystemIdentifier))
				throw new ArgumentException(string.Format("The specified subsystem '{0}' is already registered in this Context", subsystem.SubsystemIdentifier));
			
			_subsystems.Add(subsystem.SubsystemIdentifier, subsystem);
		}
		
		/// <summary>
		/// Gets a registered subsystem for the specified identifier
		/// </summary>
		protected ISubsystem GetSubsystemForIdentifier(string identifier)
		{
			if(_subsystems.ContainsKey(identifier) == false)
				return null;
			else
				return _subsystems[identifier];
		}
		
		
    }
}
