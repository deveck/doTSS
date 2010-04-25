
using System;
using Iaik.Utils.Serialization;

namespace Iaik.Utils.Hash
{


	public abstract class HashDataProvider : AutoStreamSerializable, IDisposable, ITypedStreamSerializable
	{

		/// <summary>
		/// Writes the next databytes into buffer, buffer declares the count of bytes
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns>Returns the actual written bytes</returns>
		public abstract int NextBytes(byte[] buffer);
		
		#region IDisposable implementation
		public virtual void Dispose ()
		{
		}
		
		#endregion
	}
}
