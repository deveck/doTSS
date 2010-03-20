// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.library.common;
using Iaik.Tc.Tpm.Subsystems.Tpm;

namespace Iaik.Tc.Tpm.Context
{


	/// <summary>
	/// Client used to do tpm operations
	/// </summary>
	public class TpmClient
	{
  		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private EndpointContext _ctx;

		private TpmCapabilityClient _capabilities;
		
		/// <summary>
		/// Performs tpm capability requests
		/// </summary>
		public TpmCapabilityClient Capabilities
		{
			get { return _capabilities;}
		}
		
        public TpmClient (EndpointContext ctx)
        {
        	_ctx = ctx;
        	_capabilities = new TpmCapabilityClient (this);
        }
		
		
		public void DoTPMCommandRequest (TPMCommandRequest commandRequest)
		{
			TpmRequest request = new TpmRequest (_ctx);
			request.CommandRequest = commandRequest;
			request.Execute ();
		}
		
		/// <summary>
		/// Performs tpm capability requests
		/// </summary>
		public class TpmCapabilityClient
		{
			/// <summary>
			/// Transmits the packets to the server
			/// </summary>
			private TpmClient _tpmClient;
	
	        public TpmCapabilityClient (TpmClient tpmClient)
	        {
	        	_tpmClient = tpmClient;
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
				_tpmClient.DoTPMCommandRequest (versionRequest);
				
			}
		}
	}
}