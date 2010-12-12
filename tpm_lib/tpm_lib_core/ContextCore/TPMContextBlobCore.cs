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
using Iaik.Tc.TPM.Library.Common.Context;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Library.ContextCore
{

	[TypedStreamSerializable("TPMContextBlob")]
	public class TPMContextBlobCore : TPMContextBlob, ITPMBlobReadable
	{
		public static TPMContextBlobCore CreateFromBytes(byte[] rawContextBLob)
		{
			using(TPMBlob blob = new TPMBlob(rawContextBLob))
			{
				TPMContextBlobCore contextBlob = new TPMContextBlobCore();
				contextBlob.ReadFromTpmBlob(blob);
				return contextBlob;
			}
		}

		public TPMContextBlobCore ()
		{
		}
		
		public TPMContextBlobCore(Stream src)
			:base(src)
		{
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_tag = (TPMStructureTag)blob.ReadUInt16();
			_resourceType = (TPMResourceType)blob.ReadUInt32();
			_handle = blob.ReadUInt32();
			_label = blob.ReadBytes(16);
			_contextCount = blob.ReadUInt32();
			_integrityDigest = blob.ReadBytes(20);
			
			uint additionalSize =  blob.ReadUInt32();
			_additionalData = blob.ReadBytes((int)additionalSize);
			
			uint sensitiveSize = blob.ReadUInt32();
			_sensitiveData = blob.ReadBytes((int)sensitiveSize);
		}
		
		#endregion

	}
}
