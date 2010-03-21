// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Subsystems
{


	/// <summary>
	/// Uniquely identifies a Request by the sender.
	/// This is for creating a response packet for a request packet which does not
	/// contain the packet identifier anymore
	/// </summary>
	public class PacketIdentifier
	{
		private byte _packetNr;
		
		public byte PacketNr
		{
			get{ return _packetNr;}
		}
		
		public PacketIdentifier (byte packetNr)
		{
			_packetNr = packetNr;
		}
		
		
		
	}
}
