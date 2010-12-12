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
using Iaik.Tc.TPM.Library.Common.Handles;
using Iaik.Tc.TPM.Library.Common.Context;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.ContextCore
{

	[TypedStreamSerializable("ContextHandle")]
	public class ContextHandle : AutoStreamSerializable, ITPMHandle
	{
		[SerializeMe(0)]
		private byte[] _rawContextBlob;
		
		[SerializeMe(1)]
		private TPMContextBlob _contextBlob;
		
		public ContextHandle(byte[] rawContextHandle)
		{
			_rawContextBlob = rawContextHandle;
			_contextBlob = TPMContextBlobCore.CreateFromBytes(_rawContextBlob);
		}
		
		public ContextHandle(Stream src)
		{
			Read(src);
		}
		
		#region ITPMHandle implementation
		public byte[] ContextBlob 
		{
			get {return _rawContextBlob; }
		}
		
		
		public bool ForceHandle 
		{
			get { return true; }
		}
		
		
		public uint Handle 
		{
			get { return _contextBlob.ContextCount; }
			set { _contextBlob.ContextCount = value; }
		}
		
		
		public TPMResourceType ResourceType 
		{
			get { return TPMResourceType.TPM_RT_CONTEXT; }
		}
		
		#endregion
	}
}
