//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Connection.ClientConnections;
using Iaik.Tc.Tpm.Packets;

namespace Iaik.Tc.Tpm.Context
{

	/// <summary>
	/// Adds client specific subsystems to the EndpointContext
	/// </summary>
	public sealed class ClientContext : EndpointContext
	{

		private DebugClient _debugClient;

		/// <summary>
		/// Client used to generate Debug-Subsystem Requests
		/// </summary>
		public DebugClient DebugClient
		{
			get{ return _debugClient; }
		}
		
		public ClientContext (FrontEndConnection connection, PacketTransmitter packetTransmitter)
			:base(connection, packetTransmitter)
		{			
			_debugClient = new DebugClient(packetTransmitter);
		}
	}
}
