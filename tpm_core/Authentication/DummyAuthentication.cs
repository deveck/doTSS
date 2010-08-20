// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Subsystems.Authentication;
using Mono.Unix;
using Iaik.Tc.TPM.Configuration;
using Iaik.Tc.TPM.Subsystems;

namespace Iaik.Tc.TPM.Authentication
{

	/// <summary>
	/// Defines a dummy authentication mechanism where the client authenticates as a fixed user
	/// without the need to send any authentication information. ONLY FOR TESTING
	/// </summary>
	[AuthenticationSettings("dummy_auth")]
	public class DummyAuthentication : AuthenticationMechanism
	{
		protected ServerContext ServerContext
		{
			get{ return (ServerContext)_context;}
		}
		
        public override void Initialize(EndpointContext context)
        {
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
