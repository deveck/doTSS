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
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.Storage
{

	/// <summary>
	/// Real data structure that is encrypted on bind operation.
	/// The TPM assumes that the decrypted data has this structure
	/// </summary>
	[TypedStreamSerializable("TPMBoundData")]
	public class TPMBoundData : AutoStreamSerializable, ITypedParameter
	{

		
		
		/// <summary>
		/// Must be 1.1.0.0
		/// </summary>
		[SerializeMe(0)]
		protected CapabilityData.TPMVersion _version;
		
		/// <summary>
		/// Shall be TPM_PT_BIND
		/// </summary>
		[SerializeMe(1)]
		protected TPMPayloadType _payloadType;
		
		/// <summary>
		/// The bound data
		/// </summary>
		[SerializeMe(2)]
		protected byte[] _payload;
		
		public TPMBoundData ()
		{
		}
		
		public TPMBoundData(Stream src)
		{
			Read(src);
		}
		
	}
}
