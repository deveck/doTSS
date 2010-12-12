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
			HMACSecretMissing,
			
			/// <summary>
			///The keystore does not contain a key with the specified identifier 
			/// </summary>
			KeyIdentifierMissing
        }


    	protected override string InternalErrorText 
		{
    		get 
			{
    			if (_errorCode.Value == (int)ErrorCodeEnum.TPMSessionNotFound)
    				return "The session identifier is not associated with an active tpm session";
				else if(_errorCode.Value == (int)ErrorCodeEnum.HMACSecretMissing)
					return "The HMAC secret could not be retrieved!";
				else if(_errorCode.Value == (int)ErrorCodeEnum.KeyIdentifierMissing)
					return "The keystore does not contain a key with the specified identifier";
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
