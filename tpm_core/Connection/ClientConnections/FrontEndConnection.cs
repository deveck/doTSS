//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Iaik.Tc.Tpm.Connection.Packets;
using log4net;

namespace Iaik.Tc.Tpm.Connection.ClientConnections
{
    /// <summary>
    /// Implements the IFrontEndConnection interface for common usage.
    /// This class can not be instantiated, use one of the implementations
    /// </summary>
    public abstract class FrontEndConnection : Stream
    {
		/// <summary>
		/// Raised when the connection to a remote end point has been established
		/// </summary>
		public event Action<FrontEndConnection> ConnectionEstablished;
		
		/// <summary>
		/// Raised when the connection to the remote end is closed
		/// </summary>
		public event Action<FrontEndConnection> Disconnected;
		
		/// <summary>
		/// Logger
		/// </summary>
		protected ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		
        /// <summary>
        /// Locks the connection, according to microsoft lock(this) is bad practise ;-)
        /// </summary>
        protected object _syncLock = new object();

		
		/// <summary>
		/// Indicates if the Connection is connected
		/// </summary>
		public abstract bool Connected {get;}
       
        /// <summary>
        /// Connects to the remote host
        /// </summary>
        public abstract void Connect();

        /// <summary>
        /// Closes the connection to the remote host
        /// </summary>
        public abstract void Close();

		
        #region IDisposable Members

        public virtual void Dispose()
        {
			Close();
        }

        #endregion

        #region Stream overrides
		public override bool CanRead 
		{
			get { return true; }
		}
		
		public override bool CanWrite 
		{
			get { return true; } 
		}
		
		public override bool CanSeek 
		{
			get { return false; }
		}

		public override void WriteByte (byte value)
		{
			Write(new byte[]{value}, 0, 1);		
		}
		
		public override int ReadByte ()
		{
			byte[] buffer = new byte[1];
			Read(buffer, 0, 1);
			return buffer[0];
		}

		#region Unsupported
		public override long Length 
		{
			get { throw new System.NotSupportedException();}
		}

		public override long Position 
		{
			get { throw new System.NotSupportedException();}
			set { throw new System.NotSupportedException();}
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new System.NotSupportedException();
		}

		public override void SetLength (long value)
		{
			throw new System.NotSupportedException();
		}

		#endregion
		
		
		public abstract override void Flush();
		
		public abstract override void Write(byte[] buffer, int offset, int length);
		
		public abstract override int Read(byte[] buffer, int offset, int length);
	
		#endregion
		
		protected void RaiseConnectedEvent()
		{
			if(ConnectionEstablished != null)
				ConnectionEstablished(this);
		}
		
		protected void RaiseDisconnectedEvent()
		{
			if(Disconnected != null)
				Disconnected(this);
		}
    }
}
