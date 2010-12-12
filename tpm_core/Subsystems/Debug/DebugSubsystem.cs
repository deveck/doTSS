/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.Packets;
using Iaik.Tc.TPM.Context;
using System.Reflection;
using Iaik.Tc.TPM.Configuration;

namespace Iaik.Tc.TPM.Subsystems.Debug
{

	/// <summary>
	/// Implements a Subsystem only for testing 
	/// </summary>
	public class DebugSubsystem : BaseServerSubsystem<DebugSubsystem.DebugRequestsEnum>
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
		
		public DebugSubsystem(EndpointContext context, IConnectionsConfiguration config)
            : base(context, config)
		{
			_requestExecutionInfos.Add(DebugRequestsEnum.PrintOnServerConsole, 
	        	BuildRequestExecutionInfo<DebugSubsystem, RequestPrintOnServerConsole, NoResponse>(HandlePrintOnServerConsoleRequest));
			
			_requestExecutionInfos.Add(DebugRequestsEnum.PrintOnServerConsoleWithResponse,
                BuildRequestExecutionInfo<DebugSubsystem, RequestPrintOnServerConsoleWithResponse, ResponsePrintOnServerConsole>(HandlePrintOnServerConsoleWithResponseRequest));
		}
		
		
		
		public override string SubsystemIdentifier 
		{
			get { return SubsystemConstants.SUBSYSTEM_DEBUG; }
		}





        private void HandlePrintOnServerConsoleRequest(DebugSubsystem subsystem, RequestContext<RequestPrintOnServerConsole, NoResponse> requestCtx)
		{	
			
			Console.WriteLine(requestCtx.Request.Text);
		}

        private void HandlePrintOnServerConsoleWithResponseRequest(DebugSubsystem subsystem, RequestContext<RequestPrintOnServerConsoleWithResponse, ResponsePrintOnServerConsole> requestCtx)
		{	
			Console.WriteLine(requestCtx.Request.Text);
			
			ResponsePrintOnServerConsole response = requestCtx.CreateResponse();
			response.Execute();
			
		}

	}
}
