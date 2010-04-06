
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
