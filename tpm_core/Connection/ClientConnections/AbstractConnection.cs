using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Iaik.Tc.Tpm.Connection.Packets;

namespace Iaik.Tc.Tpm.Connection.ClientConnections
{
    /// <summary>
    /// Implements the IFrontEndConnection interface for common usage.
    /// This class can not be instantiated, use one of the implementations
    /// </summary>
    public abstract class AbstractConnection : IFrontEndConnection
    {
        /// <summary>
        /// Locks the connection, according to microsoft lock(this) is bad practise ;-)
        /// </summary>
        protected object _syncLock = new object();

        /// <summary>
        /// The next available packet number
        /// </summary>
        protected byte _nextPacketNum = 0;

        /// <summary>
        /// Need not to be overriden in a derived class. It should be overriden
        /// by stream based implementations because it improves internal performance. 
        /// If this stream is supplied at least the low level write methods are bypassed
        /// and the serialization is performed directly on the stream
        /// </summary>
        protected virtual Stream OutputStream
        {
            get { return null; }
        }

        /// <summary>
        /// See OutputStream
        /// </summary>
        protected virtual Stream InputStream
        {
            get { return null; }
        }

        #region IFrontEndConnection Members

        /// <summary>
        /// See <see cref="Iaik.Tc.Tpm.Connection.IFrontEndConnection"/>
        /// </summary>
        public abstract void Connect();

        /// <summary>
        /// See <see cref="Iaik.Tc.Tpm.Connection.IFrontEndConnection"/>
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Serializes the DataPacket and writes it to the output connection
        /// 
        /// See <see cref="Iaik.Tc.Tpm.Connection.IFrontEndConnection"/> for further details
        /// </summary>
        public virtual T Transmit<T>(DataPacket packet) where T : DataPacket
        {
            lock (_syncLock)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion

        #region Raw Write/Read methods
        /// <summary>
        /// Raw write method to write to the output
        /// </summary>
        /// <param name="buffer">data buffer</param>
        /// <param name="offset">offset in the data buffer</param>
        /// <param name="length">number of bytes to write</param>
        protected abstract void WriteBytes(byte[] buffer, int offset, int length);

        /// <summary>
        /// Raw read metho to read from the input
        /// </summary>
        /// <param name="buffer">buffer to read the data to. It must be large enough to hold all the read data</param>
        /// <param name="offset">offset in the buffer</param>
        /// <param name="length">number of bytes to read</param>
        /// <returns></returns>
        protected abstract int ReadBytes(byte[] buffer, int offset, int length);
        #endregion
    }
}
