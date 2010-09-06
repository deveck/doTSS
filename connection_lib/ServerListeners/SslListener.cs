// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Net.Sockets;
using Iaik.Connection.ClientConnections;
using System.Net.Security;
using System.Security;
using Iaik.Connection.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using Iaik.Utils.CommonAttributes;
using Mono.Security.Protocol.Tls;
using System.IO;

namespace Iaik.Connection.ServerListeners
{

	[ClassIdentifier("ssl_socket")]
	public class SslListener : TcpSocketListener
	{
	
		/// <summary>
		/// Serer certificate
		/// </summary>
		private string _certificateFile;
		
		/// <summary>
		/// Password of the certificate or null
		/// </summary>
		private SecureString _certificatePassword;
		
		protected override string LogDomain 
		{
			get { return "SslListener"; }
		}

		
		public SslListener(string host, int port, string certificateFile, SecureString certPassword)
			:base(host,port)
		{
			_certificateFile = certificateFile;
			_certificatePassword = certPassword;
		}
		

		public SslListener(IListenerConfiguration listenerConfig)
			:base(listenerConfig)
		{
			_certificateFile = listenerConfig.FindParameter("server_certificate");
			_certificatePassword = null;
		}
		
		protected override void CreateFrondEndConnection (Socket socket)
		{
			X509Certificate2 serverCertificate = new X509Certificate2(_certificateFile);
			
			SslServerStream secureStream = new SslServerStream(
			    new NetworkStream(socket,  true),
			    serverCertificate,
				true,
			    true,
			    SecurityProtocolType.Tls);
			secureStream.CheckCertRevocationStatus = true;
			secureStream.PrivateKeyCertSelectionDelegate += delegate (X509Certificate cert, string targetHost) 
			{
				X509Certificate2 cert2 = serverCertificate as X509Certificate2 ?? new X509Certificate2 (serverCertificate);
				return cert2 != null ? cert2.PrivateKey : null;
			};
			
			SslConnection connection = null;
			
			try
			{
				secureStream.ClientCertValidationDelegate += ValidateRemoteCertificate;			
				secureStream.Read (new byte [0], 0, 0);
			
				connection = new SslConnection(socket, secureStream);
			}
			catch(Exception e)
			{
				_logger.FatalFormat("Error negotiating ssl connection: {0}", e);
				return;
			}
			
			RaiseClientConnectedEvent(connection);
		}
		
		
		/// <summary>
		/// Validates the certificate of the remote host
		/// </summary>
		public bool ValidateRemoteCertificate(X509Certificate certificate, int[] certificateErrors)
		{
			if(certificateErrors.Length == 0)
				return true;
			
			foreach(int certErr in certificateErrors)
				_logger.Fatal(SslConnection.CertificateErrorCodeToMessage(certErr));
			
			return false;
		}	
	}
}
