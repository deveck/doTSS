// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Context;

namespace Iaik.Tc.Tpm.Subsystems
{

	/// <summary>
	/// Subsystem Request with typed response type
	/// </summary>
	public abstract class TypedSubsystemRequest<TResponse> : SubsystemRequest 
		where TResponse:SubsystemResponse
	{

		public TypedSubsystemRequest (EndpointContext ctx)
			:base(ctx)
		{
		}
		
		public override Type ResponseType 
		{
			get { return typeof(TResponse);}
		}

		public virtual TResponse TypedExecute()
		{
			return (TResponse)Execute();
		}
		
	}
}
