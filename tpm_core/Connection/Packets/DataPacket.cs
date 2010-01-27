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

namespace Iaik.Tc.Tpm.Connection.Packets
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
        private byte _packetNr = 0;

        /// <summary>
        /// 
        /// </summary>
        private string _subsystem = "non";

        /// <summary>
        /// Create an empty packet by its type
        /// </summary>
        /// <param name="dataPacketType"></param>
        /// <returns></returns>
        public static DataPacket CreateByPacketType(DataPacketTypeAttribute.DataPacketTypeEnum dataPacketType)
        {
            if (dataPacketType == DataPacketTypeAttribute.DataPacketTypeEnum.RequestPacket)
                return new RequestDataPacket();
            else if (dataPacketType == DataPacketTypeAttribute.DataPacketTypeEnum.ResponsePacket)
                return new ResponseDataPacket();
            else
                throw new ArgumentException(string.Format("PacketType '{0}' is not supported", dataPacketType));
        }


        #region IStreamSerializable Members

        public void Write(Stream sink)
        {
            
        }

        public void Read(Stream src)
        {
            
        }

        #endregion
    }
}
