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
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common.KeyData;

namespace Iaik.Tc.TPM.Library.KeyDataCore
{


	/// <summary>
	/// Describes the parameters of an RSA key
	/// </summary>
	[TypedStreamSerializable("TPMRSAKeyParams")]
	public class TPMRSAKeyParamsCore : TPMRSAKeyParams, ITPMBlobReadable, ITPMBlobWritable
	{
		public const uint DEFAULT_KEYLENGTH = 2048;
		public const uint DEFAULT_NUMPRIMES = 2;
		
		public static TPMRSAKeyParamsCore Create (uint keyLength, uint numPrimes, byte[] exponent)
		{
			TPMRSAKeyParamsCore rsaKeyParams = new TPMRSAKeyParamsCore ();
			rsaKeyParams._keyLength = keyLength;
			
			if(numPrimes == 0)
				rsaKeyParams._numPrimes = DEFAULT_NUMPRIMES;
			else
				rsaKeyParams._numPrimes = numPrimes;
			
			if (exponent == null)
				rsaKeyParams._exponent = new byte[0];
			else
				rsaKeyParams._exponent = exponent;
			
			return rsaKeyParams;
		}
		
		public static TPMRSAKeyParamsCore CreateFromTPMBlob(TPMBlob blob)
		{
			TPMRSAKeyParamsCore keyParams = new TPMRSAKeyParamsCore();
			keyParams.ReadFromTpmBlob(blob);
			return keyParams;
		}
		
		private TPMRSAKeyParamsCore()
		{
		}
		
			
		public TPMRSAKeyParamsCore (Stream src)
		{
			Read(src);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_keyLength = blob.ReadUInt32 ();
			_numPrimes = blob.ReadUInt32 ();
			
			uint expoSize = blob.ReadUInt32 ();
			_exponent = new byte[expoSize];
			blob.Read (_exponent, 0, (int)expoSize);
		}
		
		#endregion
		#region ITPMBlobWritable implementation
		public void WriteToTpmBlob (TPMBlob blob)
		{
			blob.WriteUInt32(_keyLength);
			blob.WriteUInt32(_numPrimes);
			
			blob.WriteUInt32((uint)_exponent.Length);
			blob.Write(_exponent, 0, _exponent.Length);
		}
		
		#endregion
	}
}
