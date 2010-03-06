//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>


using System;
using Iaik.Connection.ClientConnections;
using Iaik.Connection.Packets;
using Iaik.Tc.Tpm.Subsystems.Debug;
using Iaik.Tc.Tpm.Configuration;
using Iaik.Tc.Tpm.Subsystems.Authentication;

namespace Iaik.Tc.Tpm.Context
{

	/// <summary>
	/// Adds server specific subsystems to the EndpointContext
	/// </summary>
	public class ServerContext : EndpointContext
	{
		public ServerContext (FrontEndConnection connection, PacketTransmitter packetTransmitter, IConnectionsConfiguration connectionConfig)
			:base(connection, packetTransmitter)
		{

			RegisterSubsystem(new DebugSubsystem(this, connectionConfig));
            RegisterSubsystem(new AuthenticationSubsystem(this, connectionConfig));
			_configured = true;
			_configuredEvent.Set();
		}
	}
}
