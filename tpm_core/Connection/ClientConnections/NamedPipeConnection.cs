//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Pipes;

namespace Iaik.Tc.Tpm.Connection.ClientConnections
{

    /// <summary>
    /// Implements the FrontEndConnection for named pipes.
    /// This can be used on Server and client side
    /// </summary>
    public class NamedPipeConnection : FrontEndConnection
    {
        /// <summary>
        /// Indicates if the Connection was created using a preconnected PipeStream
        /// used for server PipeStreams.
        /// </summary>
        private bool _connectedOnCreation = false;

        /// <summary>
        /// The Anonymous/Named Pipe for communication
        /// </summary>
        private PipeStream _pipeStream = null;

        /// <summary>
        /// Contains the pipe name to connect to
        /// </summary>
        private string _pipeName = null;


        public NamedPipeConnection(PipeStream pipeStream)
        {
            _pipeStream = pipeStream;
            _connectedOnCreation = true;
        }

        public NamedPipeConnection(string pipeName)
        {
            _connectedOnCreation = false;
            _pipeName = pipeName;
        }


		public override bool Connected 
		{
			get{ return _pipeStream != null; }
		}

		
        public override void Connect()
        {
            if (_pipeStream == null)
            {
                NamedPipeClientStream pipeClient = new NamedPipeClientStream("localhost", _pipeName, PipeDirection.InOut);
                pipeClient.Connect();
                _pipeStream = pipeClient;
				RaiseConnectedEvent();
            }
        }

        public override void Close()
        {
            if (_pipeStream != null)
            {
                _pipeStream.Dispose();
                _pipeStream = null;
				RaiseDisconnectedEvent();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            Close();
        }

        public override void Write(byte[] buffer, int offset, int length)
        {
            AssertPipeStream();
            _pipeStream.Write(buffer, offset, length);
        }

        public override int Read(byte[] buffer, int offset, int length)
        {
            AssertPipeStream();
            return _pipeStream.Read(buffer, offset, length);
        }
		
		public override int ReadByte ()
		{
			AssertPipeStream();
			return _pipeStream.ReadByte();
		}

		public override void WriteByte (byte value)
		{
			AssertPipeStream();
			_pipeStream.WriteByte(value);
		}

		public override void Flush ()
		{
			AssertPipeStream();
			_pipeStream.Flush();
		}

		
		/// <summary>
		/// Checks if the PipeStream is available and connected
		/// </summary>
		private void AssertPipeStream()
		{
			if (_pipeStream == null)
                throw new ConnectionException("Pipe is not connected");
		}
		
    }
}
