
using System;
using Iaik.Utils.Serialization;

namespace Iaik.Utils.Hash
{

    /// <summary>
    /// Baseclass for all HashDataProviders, which provides the serialized data (byte stream) of some kind of 
    /// entity (byte array, stream, primitive type, more complex types,...)
    /// </summary>
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
