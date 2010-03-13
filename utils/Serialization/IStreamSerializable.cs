using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Iaik.Utils.Serialization
{
    /// <summary>
    /// Defines an object that can be serialized to and read from a stream
    /// </summary>
    public interface IStreamSerializable
    {
        /// <summary>
        /// Writes the object data to the stream
        /// </summary>
        /// <param name="sink"></param>
        void Write(Stream sink);

        /// <summary>
        /// Reads the object data to the stream
        /// </summary>
        /// <param name="src"></param>
        void Read(Stream src);
    }
}
