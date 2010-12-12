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



using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.Common.KeyData
{


	[TypedStreamSerializable("TPM_STORE_PUBKEY")]
	public class TPMStorePubkey : AutoStreamSerializable, ITypedParameter
	{
		/// <summary>
		/// Stores the public key
		/// </summary>
		[SerializeMe(0)]
		protected byte[] _pubkey;		
		
		public byte[] Pubkey
		{
			get { return _pubkey;}
		}
			
		
		protected TPMStorePubkey ()
		{
		}
		
		public TPMStorePubkey (Stream src)
		{
			Read (src);
		}
		
		public override string ToString ()
		{
			return ByteHelper.ByteArrayToHexString (_pubkey);
		}

	}
}
