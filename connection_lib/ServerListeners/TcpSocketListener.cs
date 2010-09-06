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
using Iaik.Utils.CommonAttributes;
using Iaik.Connection.Configuration;
using System.Net;

namespace Iaik.Connection.ServerListeners
{

	/// <summary>
	/// Implements a listener which spawns TcpSocketConnection
	/// </summary>
	[ClassIdentifier("tcp_socket")]
	public class TcpSocketListener : IFrontEndServerListener
	{		
		/// <summary>
		/// Logger
		/// </summary>
		protected ILog _logger;
		
		protected object _syncLock = new object();
		
		/// <summary>
		/// Specifies the port to listen on
		/// </summary>
		protected int _port;
		
		/// <summary>
		/// Address used to specify the interface to listen on or 0.0.0.0 for any
		/// </summary>
		protected string _listeningAddress;

		/// <summary>
		/// Specifies the endpoint to use for connecting
		/// </summary>
		protected IPEndPoint _endpoint;
		
		/// <summary>
		/// The listening socket
		/// </summary>
		protected Socket _listeningSocket = null;
		
		protected virtual string LogDomain
		{
			get{ return "TcpSocketListener"; }
		}
		
		public TcpSocketListener (string listeningAddress, int port)
		{
			_logger = LogManager.GetLogger(LogDomain);						
			_logger.Debug(string.Format("Creating TcpSocketListener with listeningAddress={0} port={1}", listeningAddress, port));
			_listeningAddress = listeningAddress;
			_port = port;
			
			if(listeningAddress == "" || listeningAddress == "0.0.0.0")
				_endpoint = new IPEndPoint(IPAddress.Any, _port);
			else if(listeningAddress == "::0")
				_endpoint = new IPEndPoint(IPAddress.IPv6Any, _port);
			else
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(listeningAddress);
				if(hostEntry == null || hostEntry.AddressList == null || hostEntry.AddressList.Length == 0)
				{
					_logger.ErrorFormat("Could not resolve address '{0}'", listeningAddress);
					throw new ConnectionException(string.Format("Could not resolve address '{0}'", listeningAddress));
				}
				
				_endpoint = new IPEndPoint(hostEntry.AddressList[0], port);
			}
		}
		
		public TcpSocketListener(IListenerConfiguration listenerConfig)
			:this(listenerConfig.FindParameter("address"),
			      int.Parse(listenerConfig.FindParameter("port")))
		{
		}
		
		#region IDisposable implementation
		public void Dispose ()
		{
			_logger.Debug(string.Format("Disposing TcpSocketListener with address={0} port={1}", _listeningAddress, _port));
			SuspendListener();
		}
		
		#endregion
		#region IFrontEndServerListener implementation
		public event ClientConnectedDelegate ClientConnected;
				
		public void Listen ()
		{
			SuspendListener();
			
			_logger.Info(string.Format("Listening for incoming connections on '{0}:{1}'", _listeningAddress, _port));
			lock(_syncLock)
			{
				_listeningSocket = new Socket(_endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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
			CreateFrondEndConnection(clientSocket);
		}
			   
		protected virtual void CreateFrondEndConnection(Socket socket)
		{
			TcpSocketConnection connection = new TcpSocketConnection(socket);			
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

		protected void RaiseClientConnectedEvent(FrontEndConnection connection)
		{
			if(ClientConnected != null)
				ClientConnected(connection);
		}
	}
}
