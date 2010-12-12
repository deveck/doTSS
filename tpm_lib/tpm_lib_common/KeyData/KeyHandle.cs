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
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Library.Common.Handles;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.KeyData
{


	public class KeyHandle : AutoStreamSerializable, ITPMHandle
	{
		public const string KEY_SRK = "srk";
	
		#region ITPMHandle implementation
		
		[SerializeMe(0)]
		private byte[] _contextBlob = null;
		
		public byte[] ContextBlob 
		{
			get { return _contextBlob; }
			set { _contextBlob = value; } 
		}
		
		[SerializeMe(1)]
		private uint _handle;
		
		public uint Handle 
		{
			get { return _handle; }
			set { _handle = value; }
		}
		
		public bool ForceHandle
		{
			get{ return false; }
		}
		
		
		public TPMResourceType ResourceType 
		{
			get { return TPMResourceType.TPM_RT_KEY; }
		}
		
		#endregion
		
		[SerializeMe(2)]
		private string _identifier;
		
		public string Identifier
		{
			get{ return _identifier; }
		}
		

		public KeyHandle(Stream src)
		{
			Read(src);
		}
		
		public KeyHandle (string identifier, uint keyHandle)
		{
			_identifier = identifier;
			_handle = keyHandle;
		}
		
		public override string ToString ()
		{
			return string.Format("[KeyHandle: Handle=0x{0:X}, ForceHandle={1}, ResourceType={2}, Identifier={3}]", Handle, ForceHandle, ResourceType, Identifier);
		}

	}
}
