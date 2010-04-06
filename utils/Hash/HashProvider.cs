
using System;
using System.Security.Cryptography;

namespace Iaik.Utils.Hash
{

	/// <summary>
	/// Hashes data with the specified algorithm and from the specified sources
	/// </summary>
	public class HashProvider
	{

		/// <summary>
		/// The hash algorithm instance to use
		/// </summary>
		private HashAlgorithm _hashAlgorithm;
		
		public HashProvider ()
			:this("SHA1")
		{
		}
		
		public HashProvider (string hashAlgorithm)
		{
			_hashAlgorithm = HashAlgorithm.Create (hashAlgorithm);
		}
		
		/// <summary>
		/// Performs the hashing operation. The data is applied to the hash algorithm as they come in dataProviders.
		/// </summary>
		/// <param name="dataProviders"></param>
		/// <returns> Returns the hashvalue of the specified (concatenated) dataProviders</returns>
		public byte[] Hash (params HashDataProvider[] dataProviders)
		{
			_hashAlgorithm.Initialize ();
			
			byte[] buffer = new byte[1024 * 4];
			
			foreach (HashDataProvider dataProvider in dataProviders)
			{
				int bufferLength;
				do
				{
					bufferLength = dataProvider.NextBytes (buffer);
					
					if (bufferLength > 0)
						_hashAlgorithm.TransformBlock (buffer, 0, bufferLength, buffer, 0);
				}
				while (bufferLength > 0);
			}
			
			_hashAlgorithm.TransformFinalBlock (buffer, 0, 0);
			return _hashAlgorithm.Hash;
		}
	}
}
