// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;

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
		public void GetTPMVersion ()
		{
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_VERSION_VAL);
			parameters.AddPrimitiveType ("subCap", new byte[0]);
			
			TPMCommandRequest versionRequest = new TPMCommandRequest (TPMCommandNames.TPM_CMD_GetCapability, parameters);
			TPMCommandResponse response = _tpmSession.DoTPMCommandRequest (versionRequest);
			
		}
	}
}
