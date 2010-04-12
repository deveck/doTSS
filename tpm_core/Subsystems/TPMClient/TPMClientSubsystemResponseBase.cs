using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Context;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Subsystems.TPMClient
{
    /// <summary>
    /// Base class for all responses in the tpm subsystem
    /// </summary>
    public class TPMClientSubsystemResponseBase : SubsystemResponse, IStatusIndicator
    {
        public enum ErrorCodeEnum : int
        {
			/// <summary>
			///The received session identifier is not associated with an active tpm session 
			/// </summary>
			TPMSessionNotFound = SubsystemResponse.CommonErrorCodes.CustomErrorCodeBase + 1,
			
			/// <summary>
			/// The secret for hmac generation was not entered correctly
			/// </summary>
			HMACSecretMissing
        }


    	protected override string InternalErrorText 
		{
    		get 
			{
    			if (_errorCode.Value == (int)ErrorCodeEnum.TPMSessionNotFound)
    				return "The session identifier is not associated with an active tpm session";
				else if(_errorCode.Value == (int)ErrorCodeEnum.HMACSecretMissing)
					return "The HMAC secret could not be retrieved!";
    			else
    				return null;
			}
    	}



        public TPMClientSubsystemResponseBase(SubsystemRequest request, EndpointContext ctx) 
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
