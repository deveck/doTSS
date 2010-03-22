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
	/// Client used to do tpm operations
	/// </summary>
	public class TPMClient
	{
  		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private EndpointContext _ctx;

	
		/// <summary>
		/// Lists all available TPM devices
		/// </summary>
		public string[] TPMDevices
		{
			get
			{
				ListTPMsRequest request = new ListTPMsRequest (_ctx);
				ListTPMsResponse response = request.TypedExecute ();
				
				if (response.Succeeded == false)
					throw new Exception (response.ErrorText);
				
				return response.TPMDevices;
			}
		}
		
		/// <summary>
		/// Selects the specified tpm device. 
		/// On success a TPMSession object is returned
		/// </summary>
		/// <param name="tpmIdentifier"></param>
		/// <returns></returns>
		public TPMSession SelectTPMDevice (string tpmIdentifier)
		{
			SelectTPMRequest request = new SelectTPMRequest (_ctx);
			request.TPMIdentifier = tpmIdentifier;
			
			SelectTPMResponse response = request.TypedExecute ();
			response.AssertTPMSuccess ();
			
			return new TPMSession (_ctx, response.TPMSessionIdentifier);
			
		}
		
        public TPMClient (EndpointContext ctx)
        {
        	_ctx = ctx;
        }
		
		
		
		
	}
}
