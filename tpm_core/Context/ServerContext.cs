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
using Iaik.Tc.Tpm.Authentication;
using Iaik.Tc.Tpm.Subsystems.Tpm;

namespace Iaik.Tc.Tpm.Context
{

	/// <summary>
	/// Adds server specific subsystems to the EndpointContext
	/// </summary>
	public class ServerContext : EndpointContext
	{

		protected ServerAuthenticationContext _serverAuthenticationContext = null;
		
		/// <summary>
		///Saves the authentication state of the connection client 
		/// </summary>				
		public ServerAuthenticationContext ServerAuthenticationContext
		{
			get{ return _serverAuthenticationContext;}
			set{ _serverAuthenticationContext = value;}
		}
		
		public ServerContext (FrontEndConnection connection, PacketTransmitter packetTransmitter, IConnectionsConfiguration connectionConfig)
			:base(connection, packetTransmitter)
		{

			RegisterSubsystem(new DebugSubsystem(this, connectionConfig));
            RegisterSubsystem(new AuthenticationSubsystem(this, connectionConfig));
			RegisterSubsystem(new TpmSubsystem(this, connectionConfig));
			_configured = true;
			_configuredEvent.Set();
		}
	}
	
	/// <summary>
	///Saves the authentication state of the connected clients 
	/// </summary>
	public class ServerAuthenticationContext
	{
		
		protected AuthenticationMechanism _authenticationMechanism = null;
		
		/// <summary>
		///Gets or sets the authentication mechanism used by the connected client 
		/// </summary>
		public AuthenticationMechanism AuthenticationMechanism
		{
			get{ return _authenticationMechanism; }
			set{ _authenticationMechanism = value;}
		}
		
		protected IPermissionMember _authenticatedPermissionMember = null;
		
		/// <summary>
		///Sets or Gets the authenticated permission member. This is used to check permissions. 
		/// </summary>
		public IPermissionMember AuthenticatedPermissionMember
		{
			get{ return _authenticatedPermissionMember;}
			set{ _authenticatedPermissionMember = value;}
		}
	}
}
