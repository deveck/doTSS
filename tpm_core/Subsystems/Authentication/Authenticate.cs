// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.Tpm.Context;
using Iaik.Utils;
using Iaik.Tc.Tpm.Configuration;
using Iaik.Tc.Tpm.Authentication;
using System.Collections.Generic;

namespace Iaik.Tc.Tpm.Subsystems.Authentication
{
	/// <summary>
	/// Starts the authentication process using the selected AuthenticationMechanism
	/// </summary>
	public class AuthenticateRequest : TypedSubsystemRequest<AuthenticateResponse>
	{
		
		public override string Subsystem 
		{
			get { return SubsystemConstants.SUBSYSTEM_AUTH; }
		}

		public override ushort RequestIdentifier
		{
			get
			{
				return (ushort)AuthenticationSubsystem.AuthenticationRequests.Authenticate;
			}
		}
					

		public AuthenticateRequest (EndpointContext ctx)
			:base(ctx)
		{
		}
	}
	
	
	/// <summary>
	/// Response to a <see>AuthenticateRequest</see>,
	/// sends back the status of an authentication process.
	/// </summary>
	/// <remarks>
	/// This response can be extended to send an AuthenticationContext identifier. So
	/// more that one authentication sessions could be active at the same time 
	///</remarks>
	public class AuthenticateResponse : AuthenticationSubsystemResponseBase
	{	
		public AuthenticateResponse(SubsystemRequest request, EndpointContext ctx) 
			:base(request, ctx)
		{
		}		
	}
}
