// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.IO;
using System.Text;

namespace Iaik.Utils.IO
{
    /// <summary>
    /// Stream implementation which writes to a text writer on flush
    /// </summary>
	public class TextWriterStream : Stream
	{

		/// <summary>
		/// The reader to read from
		/// </summary>
		private MemoryStream _textStream = new MemoryStream();

		private TextWriter _textWriter;

		public TextWriterStream (TextWriter textWriter)
		{
			_textWriter = textWriter;
			
		}
		
		public override bool CanRead 
		{
			get { return false; }
		}
		
		public override bool CanWrite 
		{
			get { return true; }
		}
		
		public override bool CanSeek 
		{
			get { return false; }
		}
		
		public override long Length 
		{
			get { return _textStream.Length; }
		}

		public override long Position 
		{
			get { return _textStream.Position; }
			set { throw new System.NotImplementedException (); }
		}


		public override void Write (byte[] buffer, int offset, int count)
		{
			_textStream.Write(buffer, offset, count);
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			throw new System.NotImplementedException ();
		}

		public override void Flush ()
		{
			_textWriter.Write(Encoding.UTF8.GetString(_textStream.ToArray()));		
			_textStream.Flush();
		}
	
		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new System.NotImplementedException ();
		}

		public override void SetLength (long value)
		{
			throw new System.NotImplementedException ();
		}


		public override void Close ()
		{
			Flush();
			_textStream.Close();
			_textStream.Dispose();
		}

        protected override void Dispose(bool disposing)
		{
			Close();
		}
	}
}
