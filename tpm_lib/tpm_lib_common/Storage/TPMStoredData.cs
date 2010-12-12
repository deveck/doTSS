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

	[TypedStreamSerializable("TPMStoredData")]
	public class TPMStoredData : AutoStreamSerializable, ITypedParameter
	{

		/// <summary>
		/// This must be 1.1.0.0
		/// </summary>
		[SerializeMe(0)]
		protected CapabilityData.TPMVersion _versionStruct;
			
		
		[SerializeMe(1)]
		protected byte[] _sealInfo;	
		
		/// <summary>
		/// Contains pcr information to which the data is bound to
		/// </summary>
		public byte[] SealInfo
		{
			get{ return _sealInfo; }
		}
		
		/// <summary>
		/// Contains the confidential part of the data
		/// </summary>
		[SerializeMe(2)]
		protected byte[] _encData;
		

		protected TPMStoredData ()
		{
		}
		
		public TPMStoredData(Stream src)
		{
			Read(src);
		}
	}
}
