
using System;
using System.IO;
using Iaik.Utils.Serialization;

namespace Iaik.Utils.Hash
{

	/// <summary>
	/// Provides data for hasher for (seekable) streams
	/// </summary>
	[TypedStreamSerializable("h_stream_dp")]
	public class HashStreamDataProvider : HashDataProvider
	{

		/// <summary>
		/// Source stream to read from
		/// </summary>
		[SerializeMe(0)]
		private Stream _source;
		
		/// <summary>
		/// Current position inside the stream
		/// </summary>
		[SerializeMe(1)]
		private long _currentPosition;
		
		/// <summary>
		/// Length to read from the stream
		/// </summary>
		[SerializeMe(2)]
		private long _length;
		
		/// <summary>
		/// If set, destroys the underlying stream on disposing
		/// </summary>
		private bool _streamOwner = false;
		
				
		public HashStreamDataProvider (Stream src, long? startPosition, long? length, bool streamOwner)
		{
			_streamOwner = streamOwner;
			_source = src;
			
			if (startPosition == null)
				_currentPosition = 0;
			else
				_currentPosition = startPosition.Value;
			
			if (length == null)
				_length = _source.Length - _currentPosition;
			else
				_length = length.Value;
		}
		
//		public HashStreamDataProvider(Stream src)
//		{
//			Read(src);
//			_streamOwner = true;
//		}
		
		public override int NextBytes (byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException ("buffer", "cannot be null");
			
			if (buffer.Length == 0)
				throw new ArgumentException ("Short buffer");
			
			if (_length <= 0)
				return 0;
			
			int toRead = (int)Math.Min (buffer.Length, _length);
			
			_source.Position = _currentPosition;			
			int read = _source.Read (buffer, 0, toRead);
			
			_length -= read;
			_currentPosition = _source.Position;
			return read;
		}
		
		public override void Dispose ()
		{
			base.Dispose ();
			
			if (_streamOwner)
				_source.Dispose ();
		}


		public override void Write (Stream sink)
		{
			_source.Position = _currentPosition;
			base.Write(sink);
		}

		
	}
}
