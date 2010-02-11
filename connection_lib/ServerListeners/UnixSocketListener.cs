// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Mono.Unix;
using System.Net.Sockets;
using System.IO;
using Iaik.Connection.ClientConnections;
using Iaik.Connection.Packets;
using log4net;

namespace Iaik.Connection.ServerListeners
{

	/// <summary>
	/// Implements a listener which spawns UnixSocketConnections
	/// </summary>
	public class UnixSocketListener : IFrontEndServerListener
	{		
		/// <summary>
		/// Logger
		/// </summary>
		private ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		private object _syncLock = new object();
		
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
		private Socket _listeningSocket = null;
		
		public UnixSocketListener (string socketFile)
		{
			_logger.Debug(string.Format("Creating UnixSocketListener with socketFile={0}", socketFile));
			_socketFile = socketFile;
			_endpoint = new UnixEndPoint(socketFile);
		}
		
		#region IDisposable implementation
		public void Dispose ()
		{
			_logger.Debug(string.Format("Disposing UnixSocketListener with socketFile={0}", _socketFile));
			SuspendListener();
		}
		
		#endregion
		#region IFrontEndServerListener implementation
		public event ClientConnectedDelegate ClientConnected;
				
		public void Listen ()
		{
			SuspendListener();
			
			_logger.Info(string.Format("Listening for incoming connections on socketFile '{0}'", _socketFile));
			lock(_syncLock)
			{
				//TODO: currently the socket file is deleted on every startup,
				// if 2 servers are running they take ownership of the other socket file
				try{
				File.Delete(_socketFile);
				}
				catch(Exception)
				{
				}
				
				_listeningSocket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
				_listeningSocket.Bind(_endpoint);
				_listeningSocket.Listen(10);
				_listeningSocket.BeginAccept(AcceptCallback, null);
			}
		}
		
		private void AcceptCallback(IAsyncResult ar)
		{
			Socket clientSocket;
			lock(_syncLock)
			{
				//Something went wrong
				if(_listeningSocket == null)
					return;
				
				clientSocket  = _listeningSocket.EndAccept(ar);
				_listeningSocket.BeginAccept(AcceptCallback, null);
			}
			
			_logger.Info("Accepted connection");
			UnixSocketConnection connection = new UnixSocketConnection(clientSocket);			
			RaiseClientConnectedEvent(connection);
		}
			                             
		public void SuspendListener ()
		{
			lock(_syncLock)
			{
				if(_listeningSocket != null)
				{
					_logger.Debug("Suspending listener");
					_listeningSocket.Close();
					_listeningSocket = null;
				}
			}
		}
		
		#endregion

		private void RaiseClientConnectedEvent(FrontEndConnection connection)
		{
			if(ClientConnected != null)
				ClientConnected(connection);
		}
	}
}
