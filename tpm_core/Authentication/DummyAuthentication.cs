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
using Iaik.Tc.TPM.Subsystems.Authentication;
using Mono.Unix;
using Iaik.Tc.TPM.Configuration;
using Iaik.Tc.TPM.Subsystems;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;

namespace Iaik.Tc.TPM.Authentication
{

	/// <summary>
	/// Defines a dummy authentication mechanism where the client authenticates as a fixed user
	/// without the need to send any authentication information. ONLY FOR TESTING
	/// </summary>
	[AuthenticationSettings("dummy_auth")]
	public class DummyAuthentication : AuthenticationMechanism
	{
		private string _user;
		
		protected ServerContext ServerContext
		{
			get{ return (ServerContext)_context;}
		}
		
		public DummyAuthentication(IAuthenticationMethod authConfig)
		{
			if(authConfig.IsDefined("user") == false)
				throw new ArgumentException("Dummy authentication does not define 'user'-value");
		
			_user = authConfig.GetValue("user");
			
		}
		
        public override void Initialize(EndpointContext context)
        {
			if(!(context is ServerContext))
				throw new ArgumentException("DummyAuthentication expects ServerContext");
			
            base.Initialize(context);
        }
		
		public override void Authenticate (RequestContext<AuthenticateRequest, AuthenticateResponse> requestContext)
		{
			foreach(User user in ServerContext.AccessControlList.Users)
			{
				if(user.Id == _user && user.IdType == IdTypeEnum.User)
				{
					ServerContext.ServerAuthenticationContext.AuthenticatedPermissionMember = user;
				
					AuthenticateResponse response = requestContext.CreateResponse();
					response.Succeeded = true;
					response.Execute();
					return;
				}
			}

			AuthenticateResponse errorResponse = requestContext.CreateResponse();
			errorResponse.Succeeded = false;
			errorResponse.CustomErrorMessage = "Could not retrieve credentials from requesting client!";
			errorResponse.Execute();				
	
		}

		
	}
}
