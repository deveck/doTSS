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

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{
    /// <summary>
    /// Base class for all responses in the tpm subsystem
    /// </summary>
    public class TPMSubsystemResponseBase : SubsystemResponse, IStatusIndicator
    {
        public enum ErrorCodeEnum : int
        {	
			/// <summary>
			/// The specified tpm device could not be found 
			/// </summary>
			TPMDeviceNotFound,
			
			/// <summary>
			/// The last request wanted to use a tpm identifier which was not valid (not selected) 
			/// </summary>
			TPMIdentifierNotValid,
			
			/// <summary>
			///The transmitted key identifier is not valid and can not be used
			/// </summary>
			NotAValidKeyIdentifier
        }

		protected override string InternalErrorText 
		{
			get 
			{
				if (_errorCode.Value == (int)ErrorCodeEnum.TPMDeviceNotFound)
					return "The specified TPM device could not be found";
				else if (_errorCode.Value == (int)ErrorCodeEnum.TPMIdentifierNotValid)
					return "The specified TPM identifier is not valid";
				else if(_errorCode.Value == (int)ErrorCodeEnum.NotAValidKeyIdentifier)
					return "The specified key identifier is not valid and can not be used";
				else
					return null;
			}
		}


        public TPMSubsystemResponseBase(SubsystemRequest request, EndpointContext ctx) 
			:base(request, ctx)
		{
            
		}

        /// <summary>
        /// Sets the error code to a known value
        /// </summary>
        /// <param name="errorCode"></param>
        public void SetKnownErrorCode (ErrorCodeEnum errorCode)
        {
        	_errorCode = (int)errorCode;
        }

		/// <summary>
		/// Throws an TPMRequestException if the execution was not successful 
		/// </summary>
        public void AssertTPMSuccess ()
        {
        	if (_succeeded == false)
        		throw new TPMRequestException (ErrorText);
		}

     
    }
}
