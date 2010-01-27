using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.Tpm.Connection.Packets
{
    /// <summary>
    /// Attribute to classify the packet type
    /// </summary>
    public class DataPacketTypeAttribute:Attribute
    {
        public enum DataPacketTypeEnum : byte
        {
            /// <summary>
            /// Identifies a packet as RequestPacket
            /// </summary>
            RequestPacket = 0,

            /// <summary>
            /// Identifies a packet as ResponsePacket
            /// </summary>
            ResponsePacket = 1
        }

        private DataPacketTypeEnum _dataPacketType;

        public DataPacketTypeEnum DataPacketType
        {
            get { return _dataPacketType; }
        }

        public DataPacketTypeAttribute(DataPacketTypeEnum dataPacketType)
        {
            _dataPacketType = dataPacketType;
        }
    }
}
