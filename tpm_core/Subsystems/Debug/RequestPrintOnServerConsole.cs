//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.Tpm.Subsystems.Debug
{

	/// <summary>
	/// Prints the specified text on the server console
	/// </summary>
	public class RequestPrintOnServerConsole : SubsystemRequest
	{
		private string _text;
		
		/// <summary>
		/// Specifies the text to display
		/// </summary>
		public string Text
		{
			get{ return _text;}
		}
		
		public override string Subsystem 
		{
			get { return SubsystemConstants.SUBSYSTEM_DEBUG; }
		}

		public override ushort RequestIdentifier
		{
			get{ return (ushort)DebugSubsystem.DebugRequestsEnum.PrintOnServerConsole; }
		}

		public RequestPrintOnServerConsole (string text)
		{
			_text = text;
		}
		
		public RequestPrintOnServerConsole()
		{
		}
		
		public override void Write (Stream sink)
		{
			StreamHelper.WriteString(_text, sink);
		}

		public override void Read (Stream src)
		{
			_text = StreamHelper.ReadString(src);
		}

	}
}
