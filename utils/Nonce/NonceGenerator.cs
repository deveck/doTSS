
using System;

namespace Iaik.Utils.Nonce
{


	public static class NonceGenerator
	{

		public static byte[] GenerateByteNonce (int length)
		{
			byte[] randomData = new byte[length];
			GenerateByteNonce (randomData);
			return randomData;
		}
		
		public static void GenerateByteNonce (byte[] nonce)
		{
			Random r = new Random ();
			r.NextBytes (nonce);
		}
		
	}
}
