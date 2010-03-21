using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Context;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Subsystems.Tpm
{
    /// <summary>
    /// Base class for all responses in the tpm subsystem
    /// </summary>
    public class TpmSubsystemResponseBase : SubsystemResponse, IStatusIndicator
    {
        public enum ErrorCodeEnum : int
        {
			/// <summary>
			/// No active user authentication exists 
			/// </summary>
            NotAuthenticated,
			
			/// <summary>
			/// There is an active user authentication but no permission to perform this action 
			/// </summary>
			NotPermitted
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
        		if (_errorCode == null)
        			return "No error";
				else if (_errorCode.Value == -1)
        			return CustomErrorMessage;
				else if (_errorCode.Value == (int)ErrorCodeEnum.NotAuthenticated)
        			return "No authentication information for your connection was found";
				else if (_errorCode.Value == (int)ErrorCodeEnum.NotPermitted)
        			return "You are not permitted to perform this operation";
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


        public TpmSubsystemResponseBase(SubsystemRequest request, EndpointContext ctx) 
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
