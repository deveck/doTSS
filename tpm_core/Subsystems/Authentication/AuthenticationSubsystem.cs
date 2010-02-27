// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Context;

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
		
		
		protected override SubsystemRequest CreateRequestFromIdentifier (AuthenticationRequests requestType)
		{
			throw new System.NotImplementedException();
		}
		
				
		
		public AuthenticationSubsystem (EndpointContext ctx)
			:base(ctx)
		{
			_requestExecutionInfos.Add(AuthenticationRequests.ListAuthenticationMechanisms,
			      BuildRequestExecutionInfo<ListAuthenticationMechanismsRequest, ListAuthenticationMechanismsResponse>
			                           (HandleListAuthenticationMechanisms));
		}

		
		
		private void HandleListAuthenticationMechanisms(
		        RequestContext<ListAuthenticationMechanismsRequest, ListAuthenticationMechanismsResponse> requestCtx)
		{	
			

		}
		
	}
}
