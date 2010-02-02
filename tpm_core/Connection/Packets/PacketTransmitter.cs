//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Connection.ClientConnections;
using Iaik.Tc.Tpm.Connection.Packets;
using System.Threading;
using System.Collections.Generic;

namespace Iaik.Tc.Tpm.Packets
{

	/// <summary>
	/// Sends a request packet and receives the corresponding reply packet
	/// </summary>
	/// <remarks>
	/// <para>Packet numbering is done by the PacketTransmitter and works as follows. 
	/// First the numbering is only necessary for packets requireing a response packet,
	/// for all other packets the packet number is set to 0. The packet number is a simple
	/// byte (0-254), so it is only possible to have 255 "outstanding" packages.
	/// This should be enough!
	/// </para>
	/// </remarks>
	public class PacketTransmitter : IDisposable
	{	
		/// <summary>
		/// Raised on packet receival
		/// </summary>
		public event Action<DataPacket> RequestPacketReceived;
		
		/// <summary>
		/// Specifies the maximum time to wait for a packet to transmit, before the receive process
		/// gets restarted
		/// </summary>
		private readonly int _maxPacketWaitTime = 100;
		
		/// <summary>
		/// Specifies the maximum time the receive thread waits for a connection 
		/// before he manually checks again
		/// </summary>
		private readonly int _maxReceiveThreadWaitTime = 100;
		
		/// <summary>
		/// Indicates if this object has been disposed
		/// </summary>
		private volatile bool _disposed = false;
		
		/// <summary>
		/// The connection the packets are received from/transmitted to
		/// </summary>
		private FrontEndConnection _connection;
	
		/// <summary>
		/// Contains all packets to be processed
		/// </summary>
		/// <remarks>
		/// At first sight this could be implemented using a Queue<T>
		/// but for requeue reasons (if no packet numbers are available we need to requeue the
		/// current packet to the front of the list, and that is not possible when using a queue)
		/// </remarks>
		private List<PacketInfo> _packetQueue = new List<PacketInfo>();
		
		/// <summary>
		/// Tells the worker thread that another packet wants to be transmitted
		/// </summary>
		private AutoResetEvent _queueEvent = new AutoResetEvent(false);
		
		/// <summary>
		/// Contains packets waiting for their response packets
		/// key: packet nr of request packet
		/// value: Request
		/// </summary>
		private Dictionary<byte, PacketInfo> _packetsWaitingForResponse = new Dictionary<byte, PacketInfo>();
		
		/// <summary>
		/// Thread which transmits the packets to the remote end
		/// </summary>
		private Thread _transmitThread;
		
		/// <summary>
		/// Thread which receives the packets from the remote end
		/// </summary>
		private Thread _receiveThread;
		
		/// <summary>
		/// Is used by the receive thread to wait for the connection to get usable (connected)
		/// if it is not
		/// </summary>
		private AutoResetEvent _receiveThreadWaitEvent = new AutoResetEvent(false);
		
		public PacketTransmitter (FrontEndConnection connection)
		{
			_connection = connection;
			_connection.ConnectionEstablished += OnConnectionEstablished;
			_transmitThread = new Thread(TransmitProc);
			_transmitThread.Start();
			
			_receiveThread = new Thread(ReceiveProc);
			_receiveThread.Start();
		}

		
		/// <summary>
		/// Adds the given packet to the transmit queue
		/// </summary>
		/// <param name="packetInfo">A <see cref="PacketInfo"/> </param>
		private void EnqueuePacket(PacketInfo packetInfo)
		{
			lock(_packetQueue)
			{
				_packetQueue.Add(packetInfo);
				_queueEvent.Set();
			}
		}
		
		/// <summary>
		/// Dequeues a single packet from the transmit queue or return null if no packet is on the queue
		/// </summary>
		/// <returns>
		/// A <see cref="PacketInfo"/>
		/// </returns>
		private PacketInfo DequeuePacket()
		{
			lock(_packetQueue)
			{
				if(_packetQueue.Count > 0)
				{
					PacketInfo packetInfo = _packetQueue[0];
					_packetQueue.RemoveAt(0);
					return packetInfo;
				}
				
				else
					return null;
			}
		}
		
		
		/// <summary>
		/// Receives packets from the connection stream.
		/// The classification of the packet (response or request packet) and
		/// the assignment to subsystems is done outside
		/// </summary>
		private void ReceiveProc()
		{
			while(!_disposed)
			{
				if(_connection.Connected)
				{
					DataPacket packet = new DataPacket(_connection);
					
					if(packet.IsResponse)
						HandleResponsePackets(packet);
					else
						HandleRequestPackets(packet);
					
				}
				else
					_receiveThreadWaitEvent.WaitOne(_maxReceiveThreadWaitTime);		
			}
		}

		/// <summary>
		/// Trys to find the corresponding request packet, sets this packet as response
		/// and permits the transmitter of the origin request packet to proceed
		/// </summary>
		private void HandleResponsePackets(DataPacket dataPacket)
		{
			lock(_packetsWaitingForResponse)
			{
				if(_packetsWaitingForResponse.ContainsKey(dataPacket.PacketNr) == false)
				{
					//TODO: Log the packet, there is no corresponding request packet which waits
					//for a response
				}
				else
				{
					PacketInfo requestPacket = _packetsWaitingForResponse[dataPacket.PacketNr];
					requestPacket.ResponsePacket = dataPacket;
					_packetsWaitingForResponse.Remove(dataPacket.PacketNr);
					requestPacket.PacketResponseSync.Set();
				}
			}
		}
		
		/// <summary>
		/// Handles packets that are not marked as response packets,
		/// they are simply passed to the outside of the PacketTransmitter
		/// to the target subsystem
		/// </summary>
		private void HandleRequestPackets(DataPacket dataPacket)
		{
			RaiseRequestPacketReceived(dataPacket);
		}
		
		/// <summary>
		/// The thread which transmits the packets to the remote end
		/// </summary>
		private void TransmitProc()
		{
			PacketInfo currentPacket = null;
			while(!_disposed)
			{
				while((currentPacket = DequeuePacket()) != null)
				{
					try
					{
						TransmitSinglePacket(currentPacket);
					}
					catch(NoPacketNumberException)
					{
						//TODO: Log this
						// This is not really an error, but generally this should not happen
						// 254 outstanding packages are really enough
						
						lock(_packetQueue)
						{
							_packetQueue.Insert(0, currentPacket);
						}
						break;
					}
				}
				
				_queueEvent.WaitOne(_maxPacketWaitTime);
			}
		}
		
		/// <summary>
		/// Transmits a single PacketInfo to the remote host
		/// </summary>
		/// <param name="packetInfo">
		/// A <see cref="PacketInfo"/>
		/// </param>
		private void TransmitSinglePacket(PacketInfo packetInfo)
		{
			if(packetInfo.AwaitsResponse)
			{
				lock(_packetsWaitingForResponse)
				{
					byte? packetNr = FindFreePacketNr();
					if(packetNr == null)
						throw new NoPacketNumberException();
					
					packetInfo.PacketToTransmit.PacketNr = packetNr.Value;
					
					_packetsWaitingForResponse.Add(packetInfo.PacketToTransmit.PacketNr, packetInfo);
				}
			}
			else
				packetInfo.PacketToTransmit.PacketNr = 0;
			
			//DataPacketTypeAttribute attr = DataPacketTypeAttribute.FindAttributeForInstanceThrowException(packetInfo.PacketToTransmit.GetType());
			//_connection.WriteByte((byte)attr.DataPacketType);
			packetInfo.PacketToTransmit.Write(_connection);
			
		}
		
		/// <summary>
		/// Looks for a free packet number
		/// </summary>
		/// <returns>
		/// Returns a free packet number (locking need to be done outside) or null if currently
		/// no packet number is available
		/// </returns>
		private byte? FindFreePacketNr()
		{
			for(byte i = 0; i<255; i++)
			{
				if(_packetsWaitingForResponse.ContainsKey(i) == false)
					return i;
			}
			
			return null;
		}
		
		/// <summary>
		/// Transmits the specified packet to the connected host, and waits till a response 
		/// with the specified type is received
		/// </summary>
		/// <param name="packet">A <see cref="DataPacket"/> to transmit</param>
		/// <returns>Returns the specified response</returns>
		public DataPacket TransmitWithResponse(DataPacket packet)
		{
			PacketInfo packetToTransmit = new PacketInfo(packet, true);
			EnqueuePacket(packetToTransmit);
			packetToTransmit.PacketResponseSync.WaitOne();
			return packetToTransmit.ResponsePacket;
		}
		
		/// <summary>
		/// Transmits the specified packet to the connected host.
		/// </summary>
		/// <param name="packet">A <see cref="DataPacket"/> to transmit</param>
		public void TransmitWithoutResponse(DataPacket packet)
		{
			PacketInfo packetToTransmit = new PacketInfo(packet, false);
			EnqueuePacket(packetToTransmit);
		}
		
		#region IDisposable implementation
		public void Dispose ()
		{
			_disposed = true;
		}
		
		#endregion
		
		#region Event handlers/raise
		private void OnConnectionEstablished (FrontEndConnection connection)
		{
			_receiveThreadWaitEvent.Set();
		}
		
		private void RaiseRequestPacketReceived(DataPacket dataPacket)
		{
			if(RequestPacketReceived != null)
				RequestPacketReceived(dataPacket);
		}
		#endregion
		
		/// <summary>
		/// Contains some internal packet information
		/// </summary>
		private class PacketInfo
		{
			/// <summary>
			/// Guess ;-)
			/// </summary>
			private  DataPacket _packetToTransmit;
			
			/// <summary>
			/// The Response packet
			/// </summary>
			private DataPacket _responsePacket = null;
			
		
			/// <summary>
			/// Event is set when the response packet is received
			/// </summary>
			private AutoResetEvent _packetResponseSync = new AutoResetEvent(false);
			
			/// <summary>
			/// Indicates if this Packet even awaits a ResponsePacket
			/// </summary>
			private bool _awaitsResponse;
			
			/// <summary>
			/// Indicates if any kind of timeout has been exceeded in receiving the response packet
			/// </summary>
			private bool _packetExpired = false;
			
			
			public DataPacket PacketToTransmit
			{
				get{ return _packetToTransmit;}
			}
			
			public AutoResetEvent PacketResponseSync
			{
				get{ return _packetResponseSync; }
			}
			
			public bool AwaitsResponse
			{
				get{ return _awaitsResponse; }
			}
			
			public DataPacket ResponsePacket
			{
				get
				{
					lock(this)
						return _responsePacket;
				}
				set
				{
					lock(this)
						_responsePacket = value;
				}
			}
						
			public PacketInfo(DataPacket packetToTransmit, bool awaitsResponse)
			{
				_packetToTransmit = packetToTransmit;
				_awaitsResponse = awaitsResponse;
			}
			
			public bool TrySetResponsePacket(DataPacket packet)
			{
				lock(this)
				{
					if(_packetExpired) return false;
					
					_responsePacket = packet;
					return true;
				}
			}
			
			public void SetTimedOut()
			{
				lock(this)
				{
					_packetExpired = true;
				}
			}
		}
	}
}
