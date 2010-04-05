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
	/// Combines all operations that can be performed on a tpm.
	/// To retrieve a tpm session call the select method on the tpm client
	/// </summary>
	public class TPMSession
	{
		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private EndpointContext _ctx;

		/// <summary>
		/// Contains the unique session identifier of the tom session
		/// </summary>
		private int _sessionIdentifier;		
		
		private TPMCapabilityClient _capabilityClient;
		
		/// <summary>
		/// Returns the capability client of this TPMSession
		/// </summary>
		public TPMCapabilityClient CapabilityClient
		{
			get { return _capabilityClient; }
		}		
		
		public TPMSession (EndpointContext ctx, int sessionIdentifier)
		{
			_ctx = ctx;
			_sessionIdentifier = sessionIdentifier;
			_capabilityClient = new TPMCapabilityClient (this);
		}
		
		internal TPMCommandResponse DoTPMCommandRequest (TPMCommandRequest commandRequest)
		{
			TPMRequest request = new TPMRequest (_ctx);
			request.TPMIdentifier = _sessionIdentifier;
			request.CommandRequest = commandRequest;
			
			TPMResponse response = request.TypedExecute ();
			response.AssertTPMSuccess ();
			return response.CommandResponse;
		}

	}
}
