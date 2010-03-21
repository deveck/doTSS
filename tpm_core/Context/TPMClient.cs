// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Subsystems.Tpm;

namespace Iaik.Tc.TPM.Context
{


	/// <summary>
	/// Client used to do tpm operations
	/// </summary>
	public class TPMClient
	{
  		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private EndpointContext _ctx;

		private TPMCapabilityClient _capabilities;
		
		/// <summary>
		/// Performs tpm capability requests
		/// </summary>
		public TPMCapabilityClient Capabilities
		{
			get { return _capabilities;}
		}
		
		public string[] TPMDevices
		{
			get
			{
				ListTPMsRequest request = new ListTPMsRequest (_ctx);
				ListTPMsResponse response = request.TypedExecute();
				
				if (response.Succeeded == false)
					throw new Exception (response.ErrorText);
				
				return response.TPMDevices;
			}
		}
		
        public TPMClient (EndpointContext ctx)
        {
        	_ctx = ctx;
        	_capabilities = new TPMCapabilityClient (this);
        }
		
		
		public void DoTPMCommandRequest (TPMCommandRequest commandRequest)
		{
			TPMRequest request = new TPMRequest (_ctx);
			request.CommandRequest = commandRequest;
			request.Execute ();
		}
		
		/// <summary>
		/// Performs tpm capability requests
		/// </summary>
		public class TPMCapabilityClient
		{
			/// <summary>
			/// Transmits the packets to the server
			/// </summary>
			private TPMClient _tpmClient;
	
	        public TPMCapabilityClient (TPMClient tpmClient)
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
