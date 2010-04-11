
using System;
using System.IO;

namespace Iaik.Utils.Hash
{

	/// <summary>
	/// Provides data for hasher for (seekable) streams
	/// </summary>
	public class HashStreamDataProvider : HashDataProvider
	{

		/// <summary>
		/// Source stream to read from
		/// </summary>
		private Stream _source;
		
	
		/// <summary>
		/// Length to read from the stream
		/// </summary>
		private long _length;
		
		/// <summary>
		/// If set, destroys the underlying stream on disposing
		/// </summary>
		private bool _streamOwner = false;
		
		public HashStreamDataProvider (Stream src)
			:this(src, null, null, false)
		{
		}
		
		public HashStreamDataProvider (Stream src, long? startPosition, long? length, bool streamOwner)
		{
			_streamOwner = streamOwner;
			_source = src;
			
			if (startPosition == null)
				src.Position = 0;
			else
				src.Position = startPosition.Value;
			
			if (length == null)
				_length = _source.Length - src.Position;
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
			
			int toRead = (int)Math.Min (buffer.Length, _length);
			
			int read = _source.Read (buffer, 0, toRead);
			
			_length -= read;
			
			return read;
		}
		
		public override void Dispose ()
		{
			base.Dispose ();
			
			if (_streamOwner)
				_source.Dispose ();
		}


	}
}
