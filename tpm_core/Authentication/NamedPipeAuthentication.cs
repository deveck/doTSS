// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.Tpm.Context;
using Iaik.Tc.Tpm.Subsystems;
using Iaik.Tc.Tpm.Subsystems.Authentication;

namespace Iaik.Tc.Tpm.Authentication
{

	/// <summary>
	/// Defines the implicit authentication mechanism for
	/// UnixSocketConnections.
	/// If the supplied connection is not a <see>UnixSocketConnection</see>
	/// an <see>ArgumentException</see> is thrown.
	/// </summary>
	[AuthenticationSettings("named_pipe_auth", typeof(NamedPipeConnection))]
	public sealed class NamedPipeAuthentication : AuthenticationMechanism
	{
        public override void Initialize(EndpointContext context)
        {
            if (typeof(NamedPipeConnection).IsAssignableFrom(context.Connection.GetType()) == false)
                throw new ArgumentException("Supplied connection is not a NamedPipeConnection");    
        
            base.Initialize(context);
        }
		
		public override void Authenticate (RequestContext<AuthenticateRequest, AuthenticateResponse> requestContext)
		{
			throw new NotImplementedException();
		}
		
	}
}
