
using System;

namespace Iaik.Utils.Nonce
{


	public static class NonceGenerator
	{

		public static byte[] GenerateByteNonce (int length)
		{
			byte[] randomData = new byte[length];
			Random r = new Random ();
			r.NextBytes (randomData);
			
			return randomData;
		}
		
	}
}
