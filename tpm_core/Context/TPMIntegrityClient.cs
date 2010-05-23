// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Subsystems.TPMSubsystem;

namespace Iaik.Tc.TPM.Context
{


	public class TPMIntegrityClient
	{
		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private TPMSession _tpmSession;

		public TPMIntegrityClient (TPMSession tpmSession)
		{
			_tpmSession = tpmSession;
		}

		
		/// <summary>
		/// Retrieves the index of the specified pcr 
		/// </summary>
		/// <param name="pcrIndex"></param>
		/// <returns></returns>
		public byte[] PCRValue(uint pcrIndex)
		{
			Parameters pcrParams = new Parameters();
			pcrParams.AddPrimitiveType("pcrnum", pcrIndex);
		
			return BuildDoVerifyRequest(TPMCommandNames.TPM_CMD_PCRRead, pcrParams).Parameters.GetValueOf<byte[]>("value");
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
