
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
			rsaKeyParams._numPrimes = numPrimes;
			
			if (exponent == null)
				rsaKeyParams._exponent = new byte[0];
			else
				rsaKeyParams._exponent = exponent;
			
			return rsaKeyParams;
		}
		
		private TPMRSAKeyParamsCore()
		{
		}
		
			
		public TPMRSAKeyParamsCore (TPMBlob src)
		{
			ReadFromTpmBlob (src);
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
