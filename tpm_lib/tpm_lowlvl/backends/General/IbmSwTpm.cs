// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using Iaik.Utils;
using System.Net.Sockets;
using Iaik.Tc.TPM.Lowlevel.Exceptions;
using System.Threading;

namespace Iaik.Tc.TPM.Lowlevel.Backends.General
{
	/// <summary>
	/// Connects to a ibm software tpm,
	/// project page at: http://ibmswtpm.sourceforge.net/
	/// </summary>
	/// <remarks>
	/// Currently the provider only supports tcp socket connection,
	/// but it should not be that complicated to extend it to use
	/// the other providers (unix sockets,...)
	/// </remarks>
	[TPMProvider("general/ibmswtpm")]
	public class IbmSwTpm : TPMProvider
	{
		/// <summary>
		/// Remote tcp port where ibmswtpm is listening
		/// </summary>
		private int _remotePort;
		
		/// <summary>
		/// Remote tcp host where ibmswtpm is listening
		/// </summary>
		private string _remoteHost;
		
		private TcpClient _connection = new TcpClient();
		
		protected override string BackendIdentifier 
		{
			get { return "IBM SW Tpm"; }
		}

		
		public IbmSwTpm (IDictionary<string, string> parameters)
			:base(DictionaryHelper.GetBool("debug", parameters, false))
		{
			SetupLogger();
			_remotePort = DictionaryHelper.GetInt("port", parameters, 1234);	
			_remoteHost = DictionaryHelper.GetString("host", parameters, "localhost");
			
			
			//_connection.ReceiveTimeout = 20000;
		}
		
		protected override void tpmOpen ()
		{
			if(_connection.Client == null || _connection.Connected == false)
			{
				_connection = new TcpClient();
				_connection.Connect(_remoteHost, _remotePort);
			}
				
		}

		protected override void tpmClose ()
		{
			_connection.Close();
		}

		protected override byte[] tpmTransmit (byte[] blob, int size)
		{
						
			byte[] rxheader = new byte[10];
	
			//Reopen connection, the server requires one connection per command
			tpmClose();
			tpmOpen();
			/*int txlen =*/ _connection.Client.Send(blob, size, SocketFlags.None);
	        //if (txlen < size)
		    //	throw new TPMLowLvlException("Failed to write to ibm tpm socket " + deviceName_, 2);
			
			_connection.GetStream().Flush();
			// Read the TPM header
			SocketError err;
			int rxlen = _connection.Client.Receive( rxheader, 0, rxheader.Length, SocketFlags.None, out err);
			if (rxlen < 0)
			   throw new TPMLowLvlException("Failed to read from the ibm tpm socket", 2);
	
			if (rxlen < 10)
	           throw new TPMLowLvlException("Short response (" + rxlen + " bytes) from ibm tpm socket", 3);
	
	
			// Decode the length
			int length = (rxheader[2] << 24) | (rxheader[3] << 16) | (rxheader[4] << 8) | rxheader[5];
			if (length < 10)
	           throw new TPMLowLvlException("Implausible length response (" + length + " bytes) from ibm tpm socket", 4);
	    
			// Already done
			if (length == rxheader.Length)
			  return rxheader;
	
            // Need a larger buffer ...
            byte[] payload = new byte[length - rxheader.Length];

            rxlen = _connection.Client.Receive(payload, payload.Length, SocketFlags.None);
			
			if (rxlen < (length - rxheader.Length))
	            throw new TPMLowLvlException("Short payload response (" + rxlen + " bytes ) from ibm tpm socket", 5);                

			// Assemble the full response buffer
			byte[] rsp = new byte[length];
	
			//Array.Copy(_receiveBuffer, 0, rsp, 0, rxlen);
			Array.Copy(rxheader, 0, rsp, 0, rxheader.Length);
			Array.Copy(payload, 0, rsp, rxheader.Length, payload.Length);

            return rsp;
		}

		
	}
}
