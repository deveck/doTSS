// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Utils
{

	/// <summary>
	/// Provides the ability to access individual bits
	/// and export them as a byte array.
	/// </summary>
	public class BitMap:AutoStreamSerializable
	{
		/// <summary>
		/// Contains the current state of the bitmap
		/// </summary>
		[SerializeMe(0)]
		private byte[] _data;

		
		public byte[] Data
		{
			get{ return _data; }
		}
		
		/// <summary>
		/// The bit size ceiled to a multiple of 8
		/// </summary>
		/// <param name="bitsize"></param>
		public BitMap (int bitsize)
		{
			if((bitsize % 8) > 0)
				bitsize = ((bitsize % 8) + 1) * 8;
			
			//Initialization automatically occurs to zero
			_data = new byte[bitsize / 8];			
		}
		
		public BitMap(Stream src)
		{
			Read(src);
		}
		
		/// <summary>
		/// Sets the whole bitmap to the specified value
		/// </summary>
		/// <param name="value"></param>
		public void SetBitmap(bool value)
		{
			for(int i = 0; i<_data.Length; i++)
				_data[i] = value?0xff:0;
		}
		
		/// <summary>
		/// Sets the specified bit to the specified value
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void SetBit(int index, bool value)
		{
			int byteIndex = index / 8;
			int bitIndex = index % 8;
			
			byte myVal;
			
			if(value)
				_data[byteIndex] = (byte)(_data[byteIndex] | (1<<index));
			else
				_data[byteIndex] = (byte)(_data[byteIndex] & (0xfe<<index));
		}
	}
}
