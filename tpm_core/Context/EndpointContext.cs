//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Connection.ClientConnections;
using Iaik.Tc.Tpm.Packets;
using Iaik.Tc.Tpm.Connection.Packets;
using Iaik.Tc.Tpm.Subsystems;
using System.Threading;

namespace Iaik.Tc.Tpm.Context
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
		/// Creates a ServerContext for the specified connection
		/// </summary>
		public static ServerContext CreateServerEndpointContext(FrontEndConnection connection)
		{
			PacketTransmitter packetTransmitter = new PacketTransmitter(connection);
			ServerContext ctx = new ServerContext(connection, packetTransmitter);
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
		
		
		public PacketTransmitter PacketTransmitter
		{
			get{ return _packetTransmitter;}
		}
		
		public EndpointContext(FrontEndConnection connection, PacketTransmitter packetTransmitter)
		{
			_connection = connection;
			_packetTransmitter = packetTransmitter;
			_packetTransmitter.RequestPacketReceived += OnRequestPacketReceived;
		}

		/// <summary>
		/// Is called when a new DataPacket from the remote end point receives
		/// </summary>
		private void OnRequestPacketReceived (DataPacket packet)
		{
			if(_configured == false)
				_configuredEvent.WaitOne();
				
			if(_subsystems.ContainsKey(packet.Subsystem))
				_subsystems[packet.Subsystem].HandlePacket(packet);
			else
			{
				//TODO: log this! Specified subsystem has not been found
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
