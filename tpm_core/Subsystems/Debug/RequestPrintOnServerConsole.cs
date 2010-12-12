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
using System.IO;
using Iaik.Utils;
using Iaik.Tc.TPM.Context;

namespace Iaik.Tc.TPM.Subsystems.Debug
{

	/// <summary>
	/// Prints the specified text on the server console
	/// </summary>
	public class RequestPrintOnServerConsole : NoResponseSubsystemRequest
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

		public RequestPrintOnServerConsole (string text, EndpointContext ctx)
			:base(ctx)
		{
			_text = text;
		}
		
		public RequestPrintOnServerConsole(EndpointContext ctx)
			:base(ctx)
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
