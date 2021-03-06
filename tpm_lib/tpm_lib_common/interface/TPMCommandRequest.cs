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
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.Common
{
	/// <summary>
	/// Represents the command request for a specific TPM command, that should
	/// be processed by the TPM library.
	/// </summary>
	public class TPMCommandRequest : ATPMCommandQuery
	{

		private TPMCommandRequest () : base()
		{
		}
		
		public TPMCommandRequest (Stream src)
		{
			Read (src);
		}
		
		public TPMCommandRequest(String commandIdentifier, Parameters param) : base (commandIdentifier, param)
		{
		}

		public override string ToString ()
		{
			return string.Format("[TPMCommandRequest cmd={0} id={1}]", CommandIdentifier, GetHashCode());
		}

		
	}
}
