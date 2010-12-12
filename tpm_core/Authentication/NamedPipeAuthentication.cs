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
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Subsystems;
using Iaik.Tc.TPM.Subsystems.Authentication;
using System.Security.Principal;
using System.IO.Pipes;
using Iaik.Tc.TPM.Configuration;

namespace Iaik.Tc.TPM.Authentication
{

	/// <summary>
	/// Defines the implicit authentication mechanism for
	/// NamedPipeConnections.
	/// If the supplied connection is not a <see>NamedPipeConnection</see>
	/// an <see>ArgumentException</see> is thrown.
	/// </summary>
    /// <remarks>
    /// The authentication is done via windows user impersonation, so no extra interaction with the user is neccessary
    /// </remarks>
	[AuthenticationSettings("named_pipe_auth", typeof(NamedPipeConnection))]
	public sealed class NamedPipeAuthentication : AuthenticationMechanism
	{
        private ServerContext ServerContext
        {
            get { return (ServerContext)_context; }
        }

		public NamedPipeAuthentication(IAuthenticationMethod authConfig)
		{
		}
		
        public override void Initialize(EndpointContext context)
        {
            if (typeof(NamedPipeConnection).IsAssignableFrom(context.Connection.GetType()) == false)
                throw new ArgumentException("Supplied connection is not a NamedPipeConnection");    
        
            base.Initialize(context);
        }
		
		public override void Authenticate (RequestContext<AuthenticateRequest, AuthenticateResponse> requestContext)
		{
            string remoteUser = ((NamedPipeServerStream)((NamedPipeConnection)requestContext.Context.Connection).PipeStream).GetImpersonationUserName();

            if (remoteUser == null)
            {
                AuthenticateResponse response = requestContext.CreateResponse();
                response.Succeeded = false;
                response.CustomErrorMessage = "Could not retrieve credentials from requesting client!";
                response.Execute();
            }
            else
            {
                ServerContext.ServerAuthenticationContext.AuthenticatedPermissionMember =
                    new ExternalUser(remoteUser, null);


                AuthenticateResponse response = requestContext.CreateResponse();
                response.Succeeded = true;
                response.Execute();
            }
		}
		
	}
}
