/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Connection.Packets;
using Iaik.Utils;
using Iaik.Tc.TPM.Context;
using System.Reflection;

namespace Iaik.Tc.TPM.Subsystems
{
	/// <summary>
	/// Implements request base class for all default subsystem implementations (BaseSubsystem).
	/// </summary>
	public abstract class SubsystemRequest : IStreamSerializable
	{
		/// <summary>
		/// The context this request belongs to
		/// </summary>
		protected EndpointContext _ctx;
		
		/// <summary>
		/// If we are at the packet receiver this uniquely identifies the request packet on
		/// the client and is used for proper response packet generation
		/// </summary>
		protected PacketIdentifier _packetIdentifier = null;
		
		
		public PacketIdentifier PacketIdentifier
		{
			get{ return _packetIdentifier; }
			set{ _packetIdentifier = value; }
		}
		
		public SubsystemRequest(EndpointContext ctx)
		{
			_ctx = ctx;
		}
		
		/// <summary>
		/// Converts this request to a default representation of a DataPacket
		/// </summary>
		/// <returns>
		/// A <see cref="DataPacket"/>
		/// </returns>
		public virtual DataPacket ConvertToDataPacket()
		{
			
			using(MemoryStream sink = new MemoryStream())
			{
				StreamHelper.WriteUInt16(RequestIdentifier, sink);
				Write(sink);
				
				sink.Flush();
				DataPacket packet = new DataPacket(false, Subsystem, sink.ToArray());
				return packet;
			}
			
			
			
		}

		public abstract string Subsystem{get;}
		public abstract ushort RequestIdentifier{get;}
	
		/// <summary>
		/// Gets the type of the response packet, or null if no response is sent
		/// </summary>
		public abstract Type ResponseType{get;}

		
		public SubsystemResponse CreateResponse()
		{
			if(ResponseType == typeof(NoResponse))
				return null;
			
			ConstructorInfo ctor = ResponseType.GetConstructor(new Type[]{this.GetType(), typeof(EndpointContext)});
			
			if(ctor == null)
				throw new NotSupportedException(string.Format("{0} does not have ctor(EndpointContext)", ResponseType));
			
			return (SubsystemResponse)ctor.Invoke(new object[]{this, _ctx});
		}
		
		/// <summary>
		/// Executes this Request and waits for the response (if this request awaits response)
		/// </summary>
		public virtual SubsystemResponse Execute()
		{
			if(ResponseType == null || ResponseType == typeof(NoResponse))
			{
				_ctx.PacketTransmitter.TransmitWithoutResponse(ConvertToDataPacket());
				return null;
			}
			else
			{
				DataPacket responseDataPacket = _ctx.PacketTransmitter.TransmitWithResponse(ConvertToDataPacket());
				if(responseDataPacket.IsResponse == false)
					throw new NotSupportedException("Received response packet with isResponse==false, something went wrong");
				
				using(ByteStream src = new ByteStream(responseDataPacket.Payload))
				{
					//Reads the RequestIdentifier, but this is not needed for response packets
					StreamHelper.ReadUInt16(src);
					
					SubsystemResponse response = CreateResponse();
					response.Read(src);
					return response;
				}
			}
		}
		
		#region IStreamSerializable implementation
		public virtual void Write (Stream sink)
		{
		}
		
		
		public virtual void Read (Stream src)
		{
		}
		
		#endregion
		
	}
}
