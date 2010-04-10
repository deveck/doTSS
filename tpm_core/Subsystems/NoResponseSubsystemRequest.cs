// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Context;

namespace Iaik.Tc.TPM.Subsystems
{

	/// <summary>
	/// Typed Subsystemrequest with no response
	/// </summary>
	public abstract class NoResponseSubsystemRequest : TypedSubsystemRequest<NoResponse>
	{
		public NoResponseSubsystemRequest (EndpointContext ctx)
			:base(ctx)
		{
		}
		
}
	
	public class NoResponse:SubsystemResponse
	{
		public NoResponse (SubsystemRequest request, EndpointContext ctx)
			: base(request, ctx)
		{
		}
		
		protected override string InternalErrorText 
		{
			get { return null; }
		}

	}
}
