// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Mono.Unix;
using System.Net.Sockets;
using System.IO;
using Iaik.Tc.Tpm.Connection.ClientConnections;
using Iaik.Tc.Tpm.Context;
using Iaik.Tc.Tpm.Packets;

namespace Iaik.Tc.Tpm.Connection.ServerListeners
{

	/// <summary>
	/// Implements a listener which spawns UnixSocketConnections
	/// </summary>
	public class UnixSocketListener : IFrontEndServerListener
	{		
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
			_socketFile = socketFile;
			_endpoint = new UnixEndPoint(socketFile);
		}
		
		#region IDisposable implementation
		public void Dispose ()
		{
			SuspendListener();
		}
		
		#endregion
		#region IFrontEndServerListener implementation
		public event ClientConnectedDelegate ClientConnected;
				
		public void Listen ()
		{
			SuspendListener();
			
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
			
			UnixSocketConnection connection = new UnixSocketConnection(clientSocket);
			ServerContext context = new ServerContext(connection, new PacketTransmitter(connection));
			Console.WriteLine("Client connected");
			RaiseClientConnectedEvent(context);
		}
			                             
		public void SuspendListener ()
		{
			lock(_syncLock)
			{
				if(_listeningSocket != null)
				{
					_listeningSocket.Close();
					_listeningSocket = null;
				}
			}
		}
		
		#endregion

		private void RaiseClientConnectedEvent(EndpointContext context)
		{
			if(ClientConnected != null)
				ClientConnected(context);
		}
	}
}
