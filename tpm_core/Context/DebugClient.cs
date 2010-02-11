//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.Packets;
using Iaik.Tc.Tpm.Subsystems.Debug;

namespace Iaik.Tc.Tpm.Context
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
