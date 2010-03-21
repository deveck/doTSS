//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.ClientConnections;
using Iaik.Connection.Packets;

namespace Iaik.Tc.TPM.Context
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
        /// Client used to do the server side authentication
        /// </summary>
        public AuthenticationClient AuthClient
        {
        	get { return _authClient; }
        }
		
		
		private TpmClient _tpmClient;
		
		/// <summary>
		/// Client used to do tpm operations 
		/// </summary>
		public TpmClient TpmClient
		{
			get { return _tpmClient;}
		}

		public ClientContext (FrontEndConnection connection, PacketTransmitter packetTransmitter)
			: base(connection, packetTransmitter)
		{
			_debugClient = new DebugClient (this);
			_authClient = new AuthenticationClient (this);
			_tpmClient = new TpmClient (this);
			_configured = true;
			_configuredEvent.Set();
		}
	}
}
