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

namespace Iaik.Tc.TPM.Library.Common.KeyData
{

	[TypedStreamSerializable("TPM_Key")]
	public class TPMKey : AutoStreamSerializable, ITypedParameter
	{
		
	
	
		[SerializeMe(0)]
		protected CapabilityData.TPMVersion _version;
		
		public CapabilityData.TPMVersion Version
		{
			get { return _version;}
		}
		
		[SerializeMe(1)]
		protected TPMKeyUsage _keyUsage;
		
		public TPMKeyUsage KeyUsage
		{
			get { return _keyUsage;}
		}
	
		[SerializeMe(2)]
		protected TPMKeyFlags _keyFlags;
		
		public TPMKeyFlags KeyFlags
		{
			get { return _keyFlags;}
		}
		
		[SerializeMe(3)]
		protected TPMAuthDataUsage _authDataUsage;
		
		public TPMAuthDataUsage AuthDataUsage
		{
			get { return _authDataUsage;}
		}
		
		[SerializeMe(4)]
		protected TPMKeyParams _algorithmParams;
		
		public TPMKeyParams AlgorithmParams
		{
			get { return _algorithmParams;}
		}
		
		//TODO: Include PCRInfo
		
		[SerializeMe(6)]
		protected TPMStorePubkey _pubKey;
		
		public TPMStorePubkey PubKey
		{
			get { return _pubKey;}
		}
		
		[SerializeMe(7)]
		protected byte[] _encData;

		public byte[] EncData
		{
			get { return _encData;}
		}
		
		
		protected TPMKey ()
		{
		}
		
		public TPMKey (Stream src)
		{
			Read (src);
		}
	}
}
