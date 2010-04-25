// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Security.Cryptography;

namespace Iaik.Utils.Hash
{


	public class HMACProvider : HashProvider
	{
		private HMAC HMACAlgorithm
		{
			get{ return (HMAC)_hashAlgorithm; }
		}
		
		public HMACProvider (ProtectedPasswordStorage key)
			:base("HMACSHA1")
		{
			key.DecryptHash();
			HMACAlgorithm.Key = key.HashValue;
			
		}
		
		public HMACProvider (byte[] key)
			:base("HMACSHA1")
		{
			HMACAlgorithm.Key = key;
			
		}
	}
}
