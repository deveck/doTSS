
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
	public class TPMRSAKeyParams : AutoStreamSerializable, ITypedStreamSerializable
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
