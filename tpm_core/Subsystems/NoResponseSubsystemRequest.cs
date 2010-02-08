// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Context;

namespace Iaik.Tc.Tpm.Subsystems
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
		public NoResponse(SubsystemRequest request, EndpointContext ctx)
			:base(request, ctx)
		{
		}
	}
}
