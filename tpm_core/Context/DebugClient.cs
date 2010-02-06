//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Packets;
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
		private PacketTransmitter _packetTransmitter;
		
		public DebugClient (PacketTransmitter packetTransmitter)
		{
			_packetTransmitter = packetTransmitter;
		}
		
		/// <summary>
		/// Prints the specified text on the server console, no response
		/// packet is sent. Only tests the case of simple-one-way-communication
		/// </summary>
		/// <param name="text">Text to display</param>
		public void PrintOnServerConsole(string text)
		{
			RequestPrintOnServerConsole request = new RequestPrintOnServerConsole(text);
			_packetTransmitter.TransmitWithoutResponse(request.ConvertToDataPacket());
		}
	}
}
