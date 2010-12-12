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
using Iaik.Connection.ClientConnections;
using Iaik.Connection.Packets;
using Iaik.Tc.TPM.Subsystems.Debug;
using Iaik.Tc.TPM.Configuration;
using Iaik.Tc.TPM.Subsystems.Authentication;
using Iaik.Tc.TPM.Authentication;
using Iaik.Tc.TPM.Subsystems.TPMSubsystem;
using Iaik.Tc.TPM.Library;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Context
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
			get { return _serverAuthenticationContext; }
			set { _serverAuthenticationContext = value; }
		}
		
		protected AccessControlList _accessControlList;
		
		/// <summary>
		///Provides access to users, groups and permissions
		/// </summary>
		public AccessControlList AccessControlList
		{
			get { return _accessControlList; }
		}
		
		protected IDictionary<string, TPMContext> _tpmContexts;
		
		/// <summary>
		///Provides access to all defined tpm devices 
		/// </summary>
		public IDictionary<string, TPMContext> TPMContexts
		{
			get { return _tpmContexts; }
		}
		
		
		
		public ServerContext (FrontEndConnection connection, PacketTransmitter packetTransmitter, IConnectionsConfiguration connectionConfig,
			AccessControlList acl, IDictionary<string, TPMContext> tpmContexts)
			: base(connection, packetTransmitter)
		{
			_accessControlList = acl;
			_tpmContexts = tpmContexts;

			RegisterSubsystem (new DebugSubsystem (this, connectionConfig));
			RegisterSubsystem (new AuthenticationSubsystem (this, connectionConfig));
			RegisterSubsystem (new TPMSubsystem (this, connectionConfig));
			_configured = true;
			_configuredEvent.Set ();
		}
		
		
		public bool IsCurrentUserAllowed (string subsystem, string pid)
		{
			if (ServerAuthenticationContext == null || ServerAuthenticationContext.AuthenticatedPermissionMember == null)
				return false;
			
			return AccessControlList.IsAllowed (subsystem, pid, ServerAuthenticationContext.AuthenticatedPermissionMember);
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
