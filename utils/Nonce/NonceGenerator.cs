
using System;

namespace Iaik.Utils.Nonce
{

    /// <summary>
    /// Static class used to generate nonces of variable length
    /// </summary>
	public static class NonceGenerator
	{
        /// <summary>
        /// Generates a random nonce with the specified length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
		public static byte[] GenerateByteNonce (int length)
		{
			byte[] randomData = new byte[length];
			GenerateByteNonce (randomData);
			return randomData;
		}
		
        /// <summary>
        /// Fills the specified byte array with random data
        /// </summary>
        /// <param name="nonce"></param>
		public static void GenerateByteNonce (byte[] nonce)
		{
			Random r = new Random ();
			r.NextBytes (nonce);
		}
		
	}
}
