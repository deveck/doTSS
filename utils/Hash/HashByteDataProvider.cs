
using System;

namespace Iaik.Utils.Hash
{

	/// <summary>
	/// Provides data for hasher for byte arrays
	/// </summary>
	public class HashByteDataProvider : HashDataProvider
	{
		
		/// <summary>
		/// Data to read from
		/// </summary>
		private byte[] _data;
		
		/// <summary>
		/// CurrentIndex in the data bytes
		/// </summary>
		private int _currentIndex;
		
		/// <summary>
		/// length to read
		/// </summary>
		private int _length;
		
		public HashByteDataProvider (byte[] data)
			:this(data, null, null)
		{
		}
		
		public HashByteDataProvider (byte[] data, int? startIndex, int? length)
		{
			_data = data;
			
			if (startIndex == null)
				_currentIndex = 0;
			else
				_currentIndex = startIndex.Value;
			
			if (length == null)
				_length = data.Length - _currentIndex;
			else
				_length = length.Value;
			
			
		}
		
		public override int NextBytes (byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException ("buffer", "cannot be null");
			
			if (buffer.Length == 0)
				throw new ArgumentException ("Short buffer");
			
			if (_length <= 0)
				return 0;
			
			int toRead = Math.Min (buffer.Length, _length);
			
			Array.Copy (_data, _currentIndex, buffer, 0, toRead);
			
			_length -= toRead;
			
			return toRead;
		}

	}
}
