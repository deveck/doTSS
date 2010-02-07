using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Connection.ClientConnections;

namespace Iaik.Tc.Tpm.Connection
{
    /// <summary>
    /// Base class for all connection related exceptions
    /// </summary>
    public class ConnectionException : Exception
    {
        public ConnectionException(string message)
            : base(message)
        {
          
        }
    }

    /// <summary>
    /// Is thrown if the connection to the remote host could not be established,
    /// because the remote endpoint is not available
    /// </summary>
    public class ConnectionFailureException : ConnectionException
    {
        public ConnectionFailureException(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// Is thrown if the connection is rejected by the server because of
    /// some policy settings
    /// </summary>
    public class ConnectionRejectedException : ConnectionException
    {
        public ConnectionRejectedException(string message)
            : base(message)
        {
        }
    }
	
	/// <summary>
	/// Is thrown by the connection if the connection to the client has been lost
	/// </summary>
	public class DisconnectedException : ConnectionException
	{
		public DisconnectedException()
			:base ("This was not a graceful connection shutdown")
		{
		}
	}
}
