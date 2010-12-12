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
using Iaik.Utils.Hash;

namespace Iaik.Tc.TPM.Library.Common.PCRData
{

	[TypedStreamSerializable("TPMPCRInfo")]
	public class TPMPCRInfo : AutoStreamSerializable, ITypedParameter
	{
	
		/// <summary>
		/// Contains the selected PCR registers
		/// </summary>
		[SerializeMe(0)]
		protected TPMPCRSelection _pcrSelection;
		
		
//		protected byte[] DigestAtRelease
//		{
//			get
//			{
//				return new HashProvider().Hash(new HashByteDataProvider(_pcrSelection.PcrSelection.Data));
//			}
//		}
//		
//		protected byte[] DigestAtCreation
//		{
//			get
//			{
//				return DigestAtRelease;
//			}
//		}

		protected TPMPCRInfo ()
		{
		}
		
		public TPMPCRInfo(Stream src)
		{
			Read(src);
		}
	}
}
