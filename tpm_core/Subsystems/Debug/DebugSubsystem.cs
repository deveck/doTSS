//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.Packets;
using Iaik.Tc.Tpm.Context;
using System.Reflection;

namespace Iaik.Tc.Tpm.Subsystems.Debug
{

	/// <summary>
	/// Implements a Subsystem only for testing 
	/// </summary>
	public class DebugSubsystem : BaseSubsystem<DebugSubsystem.DebugRequestsEnum>
	{
		public enum DebugRequestsEnum : ushort
		{
			/// <summary>
			/// Prints the transmitted text on the server console (log)
			/// 
			/// No Response 
			/// </summary>
			PrintOnServerConsole				= 0x0001,
			
			/// <summary>
			/// Prints the transmitted text on the server console (log)
			/// and sends a response packet back
			/// </summary>
			PrintOnServerConsoleWithResponse
		}
		
		public DebugSubsystem(EndpointContext context)
			:base (context)
		{
			_requestExecutionInfos.Add(DebugRequestsEnum.PrintOnServerConsole,  
	        	BuildRequestExecutionInfo<RequestPrintOnServerConsole, NoResponse>(HandlePrintOnServerConsoleRequest));
			
			_requestExecutionInfos.Add(DebugRequestsEnum.PrintOnServerConsoleWithResponse,  
	        	BuildRequestExecutionInfo<RequestPrintOnServerConsoleWithResponse, ResponsePrintOnServerConsole>(HandlePrintOnServerConsoleWithResponseRequest));
		}
		
		
		
		public override string SubsystemIdentifier 
		{
			get { return SubsystemConstants.SUBSYSTEM_DEBUG; }
		}		
		
		
		

			
		private void HandlePrintOnServerConsoleRequest(RequestContext<RequestPrintOnServerConsole, NoResponse> requestCtx)
		{	
			
			Console.WriteLine(requestCtx.Request.Text);
		}
		
		private void HandlePrintOnServerConsoleWithResponseRequest(RequestContext<RequestPrintOnServerConsoleWithResponse, ResponsePrintOnServerConsole> requestCtx)
		{	
			Console.WriteLine(requestCtx.Request.Text);
			
			ResponsePrintOnServerConsole response = requestCtx.CreateResponse();
			response.Execute();
			
		}

	}
}
