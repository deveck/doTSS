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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Subsystems.Authentication;

namespace Iaik.Tc.TPM.Context
{
    /// <summary>
    /// Authenticates the client on the server
    /// </summary>
    public class AuthenticationClient
    {
        /// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private EndpointContext _ctx;

        public AuthenticationClient(EndpointContext ctx)
		{
			_ctx = ctx;
		}

        /// <summary>
        /// Lists all supported and usable authentication methods
        /// </summary>
        public string[] SupportedAuthenticationMethods
        {
            get
            {
                ListAuthenticationMechanismsRequest request = new ListAuthenticationMechanismsRequest(_ctx);
                return (request.Execute() as ListAuthenticationMechanismsResponse).AuthenticationModes;
            }
        }


        /// <summary>
        /// Selects the desired authentication method on the server and responds with the status of the select operation
        /// </summary>
        /// <param name="authMethod"></param>
        /// <returns></returns>
        public IStatusIndicator SelectAuthentication(string authMethod)
        {
            SelectAuthenticationMechanismsRequest request = new SelectAuthenticationMechanismsRequest(_ctx);
            request.AuthMechanismToSelect = authMethod;
            return (request.Execute() as IStatusIndicator);

        }
		
		/// <summary>
		/// Starts the authentication process
		/// </summary>
		/// <returns></returns>
		public IStatusIndicator Authenticate()
		{
			AuthenticateRequest request = new AuthenticateRequest(_ctx);
			return (request.Execute() as IStatusIndicator);
		}
    }
}
