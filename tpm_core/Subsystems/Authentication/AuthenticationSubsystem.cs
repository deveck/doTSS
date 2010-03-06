// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Context;
using Iaik.Tc.Tpm.Configuration;

namespace Iaik.Tc.Tpm.Subsystems.Authentication
{

	/// <summary>
	/// Implements the subsystem the client uses to authenticate to the server
	/// </summary>
	public class AuthenticationSubsystem : BaseSubsystem<AuthenticationSubsystem.AuthenticationRequests>
	{
		public enum AuthenticationRequests : ushort
		{
			/// <summary>
			/// Sends all available authentication mechanisms to the requesting endpoint
			/// </summary>
			ListAuthenticationMechanisms	= 0x0001,
		}
		
		public override string SubsystemIdentifier 
		{
			get { return SubsystemConstants.SUBSYSTEM_AUTH; }
		}
		
        public AuthenticationSubsystem(EndpointContext ctx, IConnectionsConfiguration config)
			:base(ctx, config)
		{
			_requestExecutionInfos.Add(AuthenticationRequests.ListAuthenticationMechanisms,
                  BuildRequestExecutionInfo<AuthenticationSubsystem, ListAuthenticationMechanismsRequest, ListAuthenticationMechanismsResponse>
                                       (ListAuthenticationMechanismsHandler.HandleListAuthenticationMechanisms));
		}
		
	}
}
