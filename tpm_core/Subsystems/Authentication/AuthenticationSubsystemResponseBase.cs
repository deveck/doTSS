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
            AuthenticationMechanismNotAvailable = 1,
			
			/// <summary>
			/// The currenct operation requires a selected authentication mechanism
			/// </summary>
			NoAuthenticationMechanismSelected,
        }


        private bool _succeeded = true;

        /// <summary>
        /// Indicates if the response has succeeded or if it has produced an error
        /// </summary>
        public bool Succeeded
        {
            get { return _succeeded; }
            set { _succeeded = value; }
        }

        private int? _errorCode = null;

        /// <summary>
        /// The error code. If the error code is -1, then a custom message is sent right after the code in the stream
        /// The error code is only transmitted if the request did not succeed
        /// </summary>
        public int? ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        /// <summary>
        /// Trys to convert the error code to a human readable string
        /// </summary>
        public string ErrorText
        {
            get
            { 
                if(ErrorCode == null)
                    return "";
                else if(ErrorCode.Value == -1)
                    return _customErrorMessage;
                else if(_errorCode == (int)ErrorCodeEnum.AuthenticationMechanismNotAvailable)
                    return "The authentication mechanism is not available for this connection or not configured on the server!";
				else if(_errorCode == (int)ErrorCodeEnum.NoAuthenticationMechanismSelected)
					return "The current operation requires an authentication mechanism. Select one and resend the request!";
                else
                    return "Unknown error";

            }
        }

        private string _customErrorMessage = null;

        /// <summary>
        /// The custom error message, only transmitted if _errorCode is -1
        /// </summary>
        public string CustomErrorMessage
        {
            get { return _customErrorMessage; }
            set 
            {
                if(value == null)
                    _errorCode = null;
                else
                    _errorCode = -1;
                _customErrorMessage = value; 
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

        

        public override void Read(Stream src)
        {
            base.Read(src);

            _succeeded = StreamHelper.ReadBool(src);

            if (_succeeded == false)
            {
                _errorCode = StreamHelper.ReadNullableInt32(src);

                if (_errorCode != null && _errorCode.Value == -1)
                    _customErrorMessage = StreamHelper.ReadString(src);

            }
        }

        public override void Write(Stream sink)
        {
            base.Write(sink);

            StreamHelper.WriteBool(_succeeded, sink);

            if (_succeeded == false)
            {
                StreamHelper.WriteNullableInt32(_errorCode, sink);

                if (_errorCode != null && _errorCode.Value == -1)
                    StreamHelper.WriteString(_customErrorMessage, sink);
            }
        }
    }
}
