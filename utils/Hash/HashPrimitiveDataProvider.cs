
using System;

namespace Iaik.Utils.Hash
{

	/// <summary>
	/// Provides data for hasher for byte arrays
	/// </summary>
	public class HashPrimitiveDataProvider : HashDataProvider
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
		
		
		
		public HashPrimitiveDataProvider (ushort value)
		{
			_data = new byte[2];
			
			_data[0] = (byte)(value >> 8);
			_data[1] = (byte)(value);
			
			_currentIndex = 0;
			_length = _data.Length;
		}
		
		public HashPrimitiveDataProvider (uint value)
		{
			_data = new byte[4];
			
			_data[0] = (byte)(value >> 24);
			_data[1] = (byte)(value >> 16);
			_data[2] = (byte)(value >> 8);
			_data[3] = (byte)(value);
			
			_currentIndex = 0;
			_length = _data.Length;
		}
		
		public HashPrimitiveDataProvider (bool value)
		{
			_data = new byte[1];
			
			if (value)
				_data[0] = 1;
			else
				_data[0] = 0;
			
			_currentIndex = 0;
			_length = _data.Length;
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
