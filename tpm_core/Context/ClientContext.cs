//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.ClientConnections;
using Iaik.Connection.Packets;

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

        private AuthenticationClient _authClient;

        /// <summary>
        /// Carries about server side authentication
        /// </summary>
        public AuthenticationClient AuthClient
        {
            get { return _authClient; }
        }

		public ClientContext (FrontEndConnection connection, PacketTransmitter packetTransmitter)
			:base(connection, packetTransmitter)
		{			
			_debugClient = new DebugClient(this);
            _authClient = new AuthenticationClient(this);
			_configured = true;
			_configuredEvent.Set();
		}
	}
}
