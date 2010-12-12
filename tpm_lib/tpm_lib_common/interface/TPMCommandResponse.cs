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


// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using Iaik.Utils;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common
{
	public sealed class TPMCommandResponse : ATPMCommandQuery
	{
	
		bool _status = false;
		
		public bool Status
		{
			get
			{
				return _status;
			}
		}
		
		public TPMCommandResponse () : base()
		{
		}
		
		public TPMCommandResponse (Stream src)
		{
			Read (src);
		}
		
		public TPMCommandResponse(bool status, String commandIdentifier, Parameters param) : base (commandIdentifier, param)
		{
			_status = status;
		}
		
		#region IStreamSerializable implementation
		public override void Write (Stream sink)
		{
			base.Write(sink);
			StreamHelper.WriteBool (_status, sink);
		}
		
		public override void Read (Stream src)
		{
			base.Read(src);
			_status = StreamHelper.ReadBool (src);
		}
		#endregion
	}
}
