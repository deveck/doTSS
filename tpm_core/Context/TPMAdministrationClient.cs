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
using Iaik.Utils.Hash;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;

namespace Iaik.Tc.TPM.Context
{


	public class TPMAdministrationClient
	{
		/// <summary>
		/// Specifies the authorization handle to use for this command
		/// </summary>
		public const string PARAM_AUTHHANDLE = "authhandle";
		
		/// <summary>
		/// The owner authdata encrypted with pubek
		/// </summary>
		public const string PARAM_OWNERAUTH = "owner_auth";
		
		/// <summary>
		/// The srk authdata encrypted with pubek
		/// </summary>
		public const string PARAM_SRKAUTH = "srk_auth";
		
		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private TPMSession _tpmSession;

		public TPMAdministrationClient (TPMSession tpmSession)
		{
			_tpmSession = tpmSession;
		}

		
		public void ClearOwner()
		{
			BuildDoVerifyRequest(TPMCommandNames.TPM_CMD_OwnerClear, new Parameters());
		}
		
		public void TakeOwnership (ProtectedPasswordStorage ownerSecret, ProtectedPasswordStorage srkSecret)
		{
			_tpmSession.SetValue ("secret_" + TPMSession.PARAM_AUTH_OWNER, ownerSecret);
			_tpmSession.SetValue ("secret_" + TPMSession.PARAM_AUTH_SRK, srkSecret);
			
			
			IAsymmetricBlockCipher ekEncryptor = _tpmSession.EndorsementKeyHandling.PublicKey.CreateRSAEncrypter ();
			
			ownerSecret.DecryptHash ();
			byte[] encOwnerSecret = ekEncryptor.ProcessBlock (ownerSecret.HashValue, 0,  ownerSecret.HashValue.Length);
			ownerSecret.ClearHash ();
			
			srkSecret.DecryptHash ();
			byte[] encSrkSecret = ekEncryptor.ProcessBlock (srkSecret.HashValue, 0, srkSecret.HashValue.Length);
			srkSecret.ClearHash ();
			
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType (PARAM_OWNERAUTH, encOwnerSecret);
			parameters.AddPrimitiveType (PARAM_SRKAUTH, encSrkSecret);
			
			/*TPMCommandResponse response = */BuildDoVerifyRequest (TPMCommandNames.TPM_CMD_TakeOwnership, parameters);
				
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
