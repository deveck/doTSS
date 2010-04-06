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
