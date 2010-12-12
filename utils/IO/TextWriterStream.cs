/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


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
