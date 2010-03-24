// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Subsystems.TPMSubsystem;

namespace Iaik.Tc.TPM.Context
{
	/// <summary>
	/// Performs tpm capability requests
	/// </summary>
	public class TPMCapabilityClient
	{
		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private TPMSession _tpmSession;

		public TPMCapabilityClient (TPMSession tpmSession)
		{
			_tpmSession = tpmSession;
		}

		
		private TPMCommandResponse BuildDoVerifyRequest(string commandIdentifier, Parameters parameters)
		{
			TPMCommandRequest versionRequest = new TPMCommandRequest (TPMCommandNames.TPM_CMD_GetCapability, parameters);
			TPMCommandResponse response = _tpmSession.DoTPMCommandRequest (versionRequest);
			
			if (response.Status == false)
				throw new TPMRequestException ("An unknown tpm error occured");
			
			return response;
		}
		
		/// <summary>
		/// Retrieves Version information of the TPM
		/// </summary>
		public CapabilityData.TPMCapVersionInfo GetTPMVersion ()
		{
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_VERSION_VAL);
			parameters.AddPrimitiveType ("subCap", new byte[0]);
			
			return BuildDoVerifyRequest(TPMCommandNames.TPM_CMD_GetCapability, parameters).Parameters.GetValueOf<CapabilityData.TPMCapVersionInfo> (CapabilityData.PARAM_TPM_VERSION_INFO);
		}
		
		/// <summary>
		/// Retrieves the count of PCR registers
		/// </summary>
		/// <returns></returns>
		public int GetPCRCount()
		{
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY);
			parameters.AddPrimitiveType ("subCap", CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_PCR);
			
			return this.BuildDoVerifyRequest(TPMCommandNames.TPM_CMD_GetCapability, parameters)
				.Parameters.GetValueOf<int>(CapabilityData.PARAM_PROP_PCR);
		}
	}
}
