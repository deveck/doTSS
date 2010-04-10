using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Context;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Subsystems.Authentication
{
    /// <summary>
    /// Base class for all responses in the authentication subsystem
    /// </summary>
    public class AuthenticationSubsystemResponseBase : SubsystemResponse, IStatusIndicator
    {
        public enum ErrorCodeEnum : int
        {
            /// <summary>
            /// Indicates that the authentication mechanism the client trys to select is not compatible 
            /// with the current connection, or is not available on this server
            /// </summary>
            AuthenticationMechanismNotAvailable = SubsystemResponse.CommonErrorCodes.CustomErrorCodeBase + 1,
			
			/// <summary>
			/// The currenct operation requires a selected authentication mechanism
			/// </summary>
			NoAuthenticationMechanismSelected,
        }


       
        /// <summary>
        /// Trys to convert the error code to a human readable string
        /// </summary>
        protected override string InternalErrorText
        {
        	get
            {
        		if (_errorCode == (int)ErrorCodeEnum.AuthenticationMechanismNotAvailable)
        			return "The authentication mechanism is not available for this connection or not configured on the server!";
				else if (_errorCode == (int)ErrorCodeEnum.NoAuthenticationMechanismSelected)
        			return "The current operation requires an authentication mechanism. Select one and resend the request!";
        		else
        			return null;

            }
        }

        
        public AuthenticationSubsystemResponseBase(SubsystemRequest request, EndpointContext ctx) 
			:base(request, ctx)
		{
            
		}

        /// <summary>
        /// Sets the error code to a known valie
        /// </summary>
        /// <param name="errorCode"></param>
        public void SetKnownErrorCode(ErrorCodeEnum errorCode)
        {
            _errorCode = (int)errorCode;
        }
    }
}
