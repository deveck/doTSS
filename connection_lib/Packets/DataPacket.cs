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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;

namespace Iaik.Connection.Packets
{
    /// <summary>
    /// Raw Request that is sent from the client to the server, it does not need to be
    /// a TPM specific request, the subsystem it belongs to is extracted from the request
    /// </summary>
    public class DataPacket : IStreamSerializable
    {
        /// <summary>
        /// Contains a continuous packet number, which is just for 
        /// identifying the response to a request.
        /// </summary>
        protected byte _packetNr = 0;

		/// <summary>
		/// Indicates if this is a response packet
		/// </summary>
		protected bool _isResponse = false;
		
        /// <summary>
        /// Specifies the target subsystem
        /// This is only relevant for non-response packets, 
        /// response packets are assigned to their request packets by_packtNr
        /// </summary>
        protected string _subsystem;

		/// <summary>
		/// Specifies the payload of this DataPacket
		/// </summary>
		protected byte[] _payload;
	
		public bool IsResponse
		{
			get{ return _isResponse;}
		}
		
		public byte PacketNr
		{
			get{ return _packetNr; }
			set{ _packetNr = value;}
		}
		
		public byte[] Payload
		{
			get{ return _payload; }
		}
		
		public string Subsystem
		{
			get{ return _subsystem;}
		}

		public DataPacket(bool isResponse, string subsystem, byte[] payload)
		{
			_isResponse = isResponse;
			_subsystem = subsystem;
			_payload = payload;
		}
		
		public DataPacket(Stream src)
		{
			Read(src);
		}
		
        #region IStreamSerializable Members

        public virtual void Write(Stream sink)
        {
        	sink.WriteByte(_packetNr);
			StreamHelper.WriteBool(_isResponse, sink);
			StreamHelper.WriteString(_subsystem, sink);			
			StreamHelper.WriteBytesSafe(_payload, sink);
        }

        public virtual void Read(Stream src)
        {
        	_packetNr = (byte)src.ReadByte();
			_isResponse = StreamHelper.ReadBool(src);
			_subsystem = StreamHelper.ReadString(src);	
			_payload = StreamHelper.ReadBytesSafe(src);
        }

        #endregion
		
		public override string ToString ()
		{
			return string.Format("[DataPacket: IsResponse={0}, PacketNr={1}, Payload=[length={2}], Subsystem={3}]", IsResponse, PacketNr, Payload.Length, Subsystem);
		}
 
    }
}
