//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>


using System;
using Iaik.Connection.ClientConnections;
using Iaik.Connection.Packets;
using Iaik.Tc.Tpm.Subsystems.Debug;

namespace Iaik.Tc.Tpm.Context
{

	/// <summary>
	/// Adds server specific subsystems to the EndpointContext
	/// </summary>
	public class ServerContext : EndpointContext
	{
		public ServerContext (FrontEndConnection connection, PacketTransmitter packetTransmitter)
			:base(connection, packetTransmitter)
		{
			Console.WriteLine("Adding debug subsystem");
			RegisterSubsystem(new DebugSubsystem(this));
			_configured = true;
			_configuredEvent.Set();
		}
	}
}
