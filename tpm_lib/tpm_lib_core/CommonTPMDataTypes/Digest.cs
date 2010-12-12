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
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;
using System.IO;
using System.Security.Cryptography;
using Iaik.Utils.Hash;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.CommonTPMDataTypes
{

	/// <summary>
	/// Represents the result of a tpm hashing operation, the default hashing operation is SHA1
	/// </summary>
	public class Digest: ITPMBlobReadable
	{
		/// <summary>
		/// Digest data
		/// </summary>
		private byte[] _digest;
		
		/// <summary>
		/// Defines the .net hash algorithm to use
		/// </summary>
		private string _hashAlgo = "SHA1";
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="blob"></param>
		/// <param name="digestSize">Digest size in bytes</param>
		public Digest (TPMBlob blob, int digestSize)
		{
			_digest = new byte[digestSize];
			ReadFromTpmBlob (blob);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			blob.Read (_digest, 0, _digest.Length);
		}		
		#endregion
		
		
		public bool CompareTo (params HashDataProvider[] hashDataProviders)
		{
			HashProvider hasher = new HashProvider (_hashAlgo);
			
			byte[] localHash = hasher.Hash (hashDataProviders);
			
			return ByteHelper.CompareByteArrays (_digest, localHash);
		}
		
		

	}
}
