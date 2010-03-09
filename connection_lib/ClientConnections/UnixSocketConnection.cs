// 
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Mono.Unix;
using System.Net.Sockets;
using log4net;
using Iaik.Utils;

namespace Iaik.Connection.ClientConnections
{

	/// <summary>
	/// Implements the named pipe replacement for unix systems,
	/// as of Mono 2.6.1 NamedPipes are not yet supported on unix systems
	/// </summary>
	[FrontEndConnection("unix_socket")]
	public sealed class UnixSocketConnection : FrontEndConnection
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
		
		/// <summary>
		/// Returns the unix socket, used to retrieve the uid of the running process
		/// </summary>
		public Socket UnixSocket
		{
			get{ return _socket; }
		}
		
		public UnixSocketConnection (string socketFile)
		{
			_logger.Debug(string.Format("Creating UnixSocketConnection with socketFile={0}", socketFile));
			_socketFile = socketFile;
			_endpoint = new UnixEndPoint(socketFile);
		}
		
		public UnixSocketConnection(Socket socket)
		{
			_logger.Debug("Creating UnixSocketConnection with preconnected socket");
			_socket = socket;
			_createdFromSocket = true;
		}
		
		public UnixSocketConnection(CommandLineHandler.CommandLineOptions commandLine)
		{
			CommandLineHandler.CommandOption socketFileOption = commandLine.FindCommandOptionByName("SocketFile");
			
			if(socketFileOption == null || socketFileOption.OptionType != 
			   CommandLineHandler.CommandOption.CommandOptionType.Value)
				throw new ArgumentException("No socket file specified!");
			else
			{
				_socketFile = socketFileOption.Arguments[0];
				_logger.DebugFormat("Using socket file '{0}'", _socketFile);
			}
				
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
				_logger.Info(string.Format("Connecting to '{0}'", _socketFile));
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
			_logger.Info(string.Format("Closing '{0}'", _socketFile));
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
			
			int read = _socket.Receive(buffer, offset, length, SocketFlags.None);
			
			if(read == 0)
				throw new DisconnectedException();
			
			return read;
		}


	
		#endregion
		
	}
}
