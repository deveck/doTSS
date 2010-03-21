// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Context;
using System.Reflection;

namespace Iaik.Tc.TPM.Subsystems
{

	/// <summary>
	/// Capsulates the Request and the necessary informations to generate a response packet
	/// </summary>
	public class RequestContext<TRequest, TResponse>
		where  TRequest: SubsystemRequest
		where TResponse: SubsystemResponse
	{
		
		private TRequest _request;
		
		/// <summary>
		/// Context this RequestContext<,> belongs to
		/// </summary>
		private EndpointContext _ctx;
		
		/// <summary>
		/// Request receives from the remote side
		/// </summary>
		public TRequest Request
		{
			get{ return _request; }
		}			 
		
		public RequestContext (TRequest request, EndpointContext ctx)
		{
			_request = request;
			_ctx = ctx;
		}
		
		public TResponse CreateResponse()
		{
			return (TResponse)_request.CreateResponse();
		}
	}
}
