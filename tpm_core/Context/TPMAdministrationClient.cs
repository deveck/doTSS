// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Subsystems.TPMSubsystem;
using Iaik.Utils.Hash;
using System.Security.Cryptography;

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

		
		public void TakeOwnership (ProtectedPasswordStorage ownerSecret, ProtectedPasswordStorage srkSecret)
		{
			_tpmSession.SetValue (TPMSession.PARAM_AUTH_OWNER, ownerSecret);
			_tpmSession.SetValue (TPMSession.PARAM_AUTH_SRK, srkSecret);
			
			
			RSA ekEncryptor = _tpmSession.EndorsementKeyHandling.PublicKey.CreateRSAEncrypter ();
			
			ownerSecret.DecryptHash ();
			byte[] encOwnerSecret = ekEncryptor.EncryptValue (ownerSecret.HashValue);
			ownerSecret.ClearHash ();
			
			srkSecret.DecryptHash ();
			byte[] encSrkSecret = ekEncryptor.EncryptValue (srkSecret.HashValue);
			srkSecret.ClearHash ();
			
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType (PARAM_OWNERAUTH, encOwnerSecret);
			parameters.AddPrimitiveType (PARAM_SRKAUTH, encSrkSecret);
			
			TPMCommandResponse response = BuildDoVerifyRequest (TPMCommandNames.TPM_CMD_TakeOwnership, parameters);
				
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
