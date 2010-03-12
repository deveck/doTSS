// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.Tpm.Context;
using Iaik.Tc.Tpm.Subsystems.Authentication;
using Mono.Unix;
using Iaik.Tc.Tpm.Configuration;
using Iaik.Tc.Tpm.Subsystems;

namespace Iaik.Tc.Tpm.Authentication
{

	/// <summary>
	/// Defines the implicit authentication mechanism for
	/// UnixSocketConnections.
	/// If the supplied connection is not a <see>UnixSocketConnection</see>
	/// an <see>ArgumentException</see> is thrown.
	/// </summary>
	[AuthenticationSettings("unix_auth", typeof(UnixSocketConnection))]
	public class UnixSocketAuthentication : AuthenticationMechanism
	{
		protected ServerContext ServerContext
		{
			get{ return (ServerContext)_context;}
		}
		
        public override void Initialize(EndpointContext context)
        {
            if (typeof(UnixSocketConnection).IsAssignableFrom(context.Connection.GetType()) == false)
                throw new ArgumentException("Supplied connection is not a UnixSocketConnection");    
        
			if(!(context is ServerContext))
				throw new ArgumentException("UnixSocketAuthentication expects ServerContext");
			
            base.Initialize(context);
        }
		
		public override void Authenticate (RequestContext<AuthenticateRequest, AuthenticateResponse> requestContext)
		{
			PeerCred credentials = new PeerCred(((UnixSocketConnection)ServerContext.Connection).UnixSocket);
			
			if(credentials == null)
			{
				AuthenticateResponse response = requestContext.CreateResponse();
				response.Succeeded = false;
				response.CustomErrorMessage = "Could not retrieve credentials from requesting client!";
				response.Execute();				
			}			
			else
			{
				ServerContext.ServerAuthenticationContext.AuthenticatedPermissionMember =
					new ExternalUser(credentials.UserID.ToString(), credentials.GroupID.ToString());
				
				AuthenticateResponse response = requestContext.CreateResponse();
				response.Succeeded = true;
				response.Execute();
			}
		}

		
	}
}
