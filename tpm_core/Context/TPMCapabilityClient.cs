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

		/// <summary>
		/// 
		/// </summary>
		public CapabilityData.TPMCapVersionInfo GetTPMVersion ()
		{
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_VERSION_VAL);
			parameters.AddPrimitiveType ("subCap", new byte[0]);
			
			TPMCommandRequest versionRequest = new TPMCommandRequest (TPMCommandNames.TPM_CMD_GetCapability, parameters);
			TPMCommandResponse response = _tpmSession.DoTPMCommandRequest (versionRequest);
			
			if (response.Status == false)
				throw new TPMRequestException ("An unknown tpm error occured");
			
			return response.Parameters.GetValueOf<CapabilityData.TPMCapVersionInfo> (CapabilityData.PARAM_TPM_VERSION_INFO);
		}
	}
}
