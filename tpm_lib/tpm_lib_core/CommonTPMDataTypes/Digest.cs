
using System;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;
using System.IO;
using System.Security.Cryptography;

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
		
		
		public bool CompareTo (params DigestHashDestination[] hashDestinations)
		{
			HashAlgorithm hashAlgo = HashAlgorithm.Create (_hashAlgo);
			
			hashAlgo.Initialize ();
			
			return true;
			
		}
		
		

		public abstract class DigestHashDestination
		{
			/// <summary>
			/// Gets the next block of data to hash with a maximum size of buffer.length
			/// </summary>
			/// <param name="buffer"></param>
			/// <returns>Returns the actual amount of bytes written to the buffer</returns>
			public abstract int NextBlock(byte[] buffer);
		}
		
		
		public class DigestStreamHashDestination : DigestHashDestination
		{
			public override int NextBlock (byte[] buffer)
			{
				return 0;
			}
		}
	}
}
