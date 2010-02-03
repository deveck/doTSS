// 
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Mono.Unix;
using System.Net.Sockets;

namespace Iaik.Tc.Tpm.Connection.ClientConnections
{

	/// <summary>
	/// Implements the named pipe replacement for unix systems,
	/// as of Mono 2.6.1 NamedPipes are not yet supported on unix systems
	/// </summary>
	public class UnixSocketConnection : FrontEndConnection
	{
		/// <summary>
		/// Specifies the unix socket file to use
		/// </summary>
		private string _socketFile;

		/// <summary>
		/// Specifies the endpoint to use for connecting
		/// </summary>
		private UnixEndPoint _endpoint;
		
		/// <summary>
		/// The socket to connect thru
		/// </summary>
		private Socket _socket = null;
		
		/// <summary>
		/// Indicates if this Connection can be reconnected or if it 
		/// was created using a preconnected socket
		/// </summary>
		private bool _createdFromSocket = false;
		
		public UnixSocketConnection (string socketFile)
		{
			_socketFile = socketFile;
			_endpoint = new UnixEndPoint(socketFile);
		}
		
		public UnixSocketConnection(Socket socket)
		{
			_socket = socket;
			_createdFromSocket = true;
		}
		
		#region FrontEndConnection overrides
		
		public override bool Connected 
		{
			get { return _socket != null && _socket.Connected;}
		}

		
		public override void Flush ()
		{
			//No flushing required
		}

		public override void Connect()
		{
			if(_createdFromSocket)
				throw new ConnectionException("Cannot reconnect preconnected socket");
				
			if(_socket == null || _socket.Connected == false)
			{
				try
				{
					_socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
					_socket.Connect(_endpoint);
				}
				catch(Exception ex)
				{
					throw new ConnectionFailureException(ex.Message);
				}
			}
		}
		
		public override void Close ()
		{
			_socket.Close();
		}

	
		public override void Write (byte[] buffer, int offset, int length)
		{
			if(Connected == false)
				throw new ConnectionException("Socket not connected");
			
			_socket.Send(buffer, offset, length, SocketFlags.None);
		}
		
		public override int Read (byte[] buffer, int offset, int length)
		{
			if(Connected == false)
				throw new ConnectionException("Socket not connected");
			
			return _socket.Receive(buffer, offset, length, SocketFlags.None);
		}


		#endregion
		
	}
}
