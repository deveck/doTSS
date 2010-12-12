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
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Subsystems.TPMSubsystem;

namespace Iaik.Tc.TPM.Context
{


	public class TPMEndorsementKeyHandlingClient
	{
		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private TPMSession _tpmSession;

		public TPMEndorsementKeyHandlingClient (TPMSession tpmSession)
		{
			_tpmSession = tpmSession;
		}

		
		public TPMPubkey PublicKey
		{
			get
			{
				TPMCommandResponse response = BuildDoVerifyRequest (TPMCommandNames.TPM_CMD_ReadPubek, new Parameters ());
				
				return response.Parameters.GetValueOf<TPMPubkey> (TPMPubkey.PARAM_TPM_PUBKEY);
			}
		}
		
		private TPMCommandResponse BuildDoVerifyRequest (string commandIdentifier, Parameters parameters)
		{
			TPMCommandRequest versionRequest = new TPMCommandRequest (commandIdentifier, parameters);
			TPMCommandResponse response = _tpmSession.DoTPMCommandRequest (versionRequest);
			
			if (response.Status == false)
				throw new TPMRequestException ("An unknown tpm error occured");
			
			return response;
		}
	}
}
