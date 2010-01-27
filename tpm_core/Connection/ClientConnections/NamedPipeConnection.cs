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
    public class NamedPipeConnection : AbstractConnection
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

        protected override Stream InputStream
        {
            get { return _pipeStream; }
        }

        protected override Stream OutputStream
        {
            get { return _pipeStream; }
        }

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


        public override void Connect()
        {
            if (_pipeStream == null)
            {
                _pipeStream = new NamedPipeClientStream("localhost", _pipeName, PipeDirection.InOut);
            }
        }

        public override void Close()
        {
            if (_pipeStream != null)
            {
                _pipeStream.Dispose();
                _pipeStream = null;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            Close();
        }

        protected override void WriteBytes(byte[] buffer, int offset, int length)
        {
            if (_pipeStream == null)
                throw new ConnectionException("Pipe is not connected");

            _pipeStream.Write(buffer, offset, length);
        }

        protected override int ReadBytes(byte[] buffer, int offset, int length)
        {
            if (_pipeStream == null)
                throw new ConnectionException("Pipe is not connected");

            return _pipeStream.Read(buffer, offset, length);
        }
    }
}
