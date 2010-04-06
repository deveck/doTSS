
using System;

namespace Iaik.Utils.Hash
{


	public abstract class HashDataProvider
	{

		/// <summary>
		/// Writes the next databytes into buffer, buffer declares the count of bytes
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns>Returns the actual written bytes</returns>
		public abstract int NextBytes(byte[] buffer);
	}
}
