using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Subsystems.Authentication;

namespace Iaik.Tc.Tpm.Context
{
    /// <summary>
    /// Carries about authentication on the client side.
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
    }
}
