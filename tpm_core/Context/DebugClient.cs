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
using Iaik.Tc.TPM.Subsystems.Debug;

namespace Iaik.Tc.TPM.Context
{

	/// <summary>
	/// Generates requests (on the client side) for the debug subsystem
	/// </summary>
	public class DebugClient
	{
		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private EndpointContext _ctx;
		
		public DebugClient (EndpointContext ctx)
		{
			_ctx = ctx;
		}
		
		/// <summary>
		/// Prints the specified text on the server console, no response
		/// packet is sent. Only tests the case of simple-one-way-communication
		/// </summary>
		/// <param name="text">Text to display</param>
		public void PrintOnServerConsole(string text)
		{
			RequestPrintOnServerConsole request = new RequestPrintOnServerConsole(text, _ctx);
			request.Execute();
		}
		
		/// <summary>
		/// Prints the specified text on the server console, with ticks at execution of server
		/// packet is sent. Only tests the case of simple-one-way-communication
		/// </summary>
		/// <param name="text">Text to display</param>
		public long PrintOnServerConsoleWithResponse(string text)
		{
			RequestPrintOnServerConsoleWithResponse request = new RequestPrintOnServerConsoleWithResponse(text, _ctx);
			return request.TypedExecute().ServerTicks;
		}
	}
}
