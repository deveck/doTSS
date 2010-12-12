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


	/// <summary>
	/// Describes the parameters of an RSA key
	/// </summary>
	[TypedStreamSerializable("TPMRSAKeyParams")]
	public class TPMRSAKeyParams : AutoStreamSerializable, ITypedStreamSerializable, ITPMAlgorithmSpecificKeyParams
	{
		[SerializeMe(0)]
		protected uint _keyLength;
		
		/// <summary>
		/// Size of the RSA key in bits
		/// </summary>
		public uint KeyLength
		{
			get { return _keyLength; }
		}
		
		public uint InputBlockSize
		{
			get{ return _keyLength / 8; }
		}
		
		public uint OutputBlockSize
		{
			get{ return InputBlockSize; }
		}
		
		[SerializeMe(1)]
		protected uint _numPrimes;
			
		/// <summary>
		/// Number of prime factors used by this RSA key
		/// </summary>
		public uint NumPrimes
		{
			get { return _numPrimes; }
		}
		
	
		[SerializeMe(2)]
		protected byte[] _exponent;
		
		/// <summary>
		/// Exponent of this key
		/// </summary>
		public byte[] Exponent
		{
			get { return _exponent;}
		}

		/// <summary>
		/// If no exponent is supplied by the TPM the default value is used 2^16+1
		/// </summary>
		/// <returns>
		/// A <see cref="System.Byte"/>
		/// </returns>
		public byte[] GetExponent ()
		{
			if (_exponent != null && _exponent.Length > 0)
				return _exponent;
			else
			{
				int exponent = 65537;
				
				byte[] expo = new byte[4];
				
				expo[0] = ((byte)(exponent >> 24));
            	expo[1] = ((byte)(exponent >> 16));
            	expo[2] = ((byte)(exponent >> 8));
            	expo[3] = ((byte)(exponent));
            	return expo;
			}
		}
		
		protected TPMRSAKeyParams ()
		{
		}
		
		public TPMRSAKeyParams (Stream src)
		{
			Read (src);
		}
		
		public override string ToString ()
		{
			return string.Format ("KeyLength: {0} bits\nNumPrimes: {1}\nExponent: {2}", KeyLength, NumPrimes, 
				Exponent==null?"<null>":ByteHelper.ByteArrayToHexString (Exponent));
		}

	}
}
