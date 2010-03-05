// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
// Thanks to Johannes

using System;
using System.IO;

namespace Iaik.Tc.Tpm.lowlevel
{


	public class TpmBlob : MemoryStream
	{

		public TpmBlob ()
		{
		}
		
		#region Reading data
        /// <summary>
        /// Alternative ReadByte method, which throws an exception
        /// if the base class version fails.
        /// 
        /// This method hides the base-class ReadByte method (which
        /// returns in int value ...)
        /// </summary>
        /// <returns></returns>
        public new byte ReadByte()
        {
            int in_byte = base.ReadByte();
            if (in_byte < 0)
                throw new EndOfStreamException();

            return (byte)in_byte;
        }

        /// <summary>
        /// Read a boolean value from the stream.
        /// 
        /// This method raises an exception if the read byte value
        /// is neither 0x01 nor 0x00 (i.a. a valid TCG boolean value)
        /// </summary>
        /// <returns></returns>
        public bool ReadBool()
        {
            byte in_byte = ReadByte();

            if (in_byte != 0x00 && in_byte != 0x01)
                throw new InvalidDataException("Bad boolean value");

            return (in_byte == 0x01);
        }

        /// <summary>
        /// Reads a 16bit unsigned integer from the stream.
        /// </summary>
        /// <returns></returns>
        public UInt16 ReadUInt16()
        {
            UInt16 result;
            result = (UInt16)((UInt16)ReadByte() << 8);
            result |= (UInt16)ReadByte();
            return result;
        }

        /// <summary>
        /// Reads a 16bit unsigned integer from the stream.
        /// </summary>
        /// <returns></returns>
        public UInt32 ReadUInt32()
        {
            UInt32 result;
            result = ((UInt32)ReadByte() << 24);
            result |= ((UInt32)ReadByte() << 16);
            result |= ((UInt32)ReadByte() << 8);            
            result |= ReadByte();
            return result;
        }

        /// <summary>
        /// Reads a 16bit unsigned integer from the stream.
        /// </summary>
        /// <returns></returns>
        public UInt64 ReadUInt64()
        {
            UInt64 result;
            result = ((UInt64)ReadByte() << 56);
            result |= ((UInt64)ReadByte() << 48);
            result |= ((UInt64)ReadByte() << 40);
            result |= ((UInt64)ReadByte() << 32);
            result |= ((UInt64)ReadByte() << 24);
            result |= ((UInt64)ReadByte() << 16);
            result |= ((UInt64)ReadByte() << 8);
            result |= ReadByte();
            return result;
        }

        /// <summary>
        /// Extract a new byte blob from the TPM blob
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int size)
        {
            byte[] result = new byte[size];
            if (this.Read(result, 0, size) != size)
                throw new EndOfStreamException();

            return result;
        }
        #endregion
		
		 #region Writing data
        /// <summary>
        /// Write a TPM command header (with size 0, assuming that
        /// WriteCmdSize will be called later)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ordinal"></param>
        public void WriteCmdHeader(UInt16 tag, UInt32 ordinal)
        {
            WriteCmdHeader(tag, ordinal, 0);
        }

        /// <summary>
        /// Write an object to the TPM blob
        /// </summary>
        /// <param name="obj"></param>
        public void Write(ITpmBlobSerializable obj)
        {
            obj.WriteToBlob(this);
        }

        /// <summary>
        /// Write the TPM command header
        /// </summary>
        /// <param name="?"></param>
        public void WriteCmdHeader(UInt16 tag, UInt32 ordinal, int size)
        {
            WriteUInt16(tag);
            WriteUInt32((UInt32)size);
            WriteUInt32(ordinal);            
        }

        /// <summary>
        /// Backpatch the command size
        /// </summary>
        public void WriteCmdSize()
        {            
            long current = this.Position;

            Position = 2;
            WriteUInt32((UInt32)this.Length);
            Position = current;
        }

        /// <summary>
        /// Write a boolean value to the stream
        /// </summary>
        /// <param name="b"></param>
        public void WriteBool(bool b)
        {
            WriteByte(b ? (byte)0x01 : (byte)0x00);
        }

        /// <summary>
        /// Write a 16 bit unsigned integer to the stream.
        /// </summary>
        /// <param name="value"></param>
        public void WriteUInt16(UInt16 value)
        {
            WriteByte((byte)(value >> 8));
            WriteByte((byte)(value));
        }

        /// <summary>
        /// Write a 32 bit unsigned integer to the stream.
        /// </summary>
        /// <param name="value"></param>
        public void WriteUInt32(UInt32 value)
        {
            WriteByte((byte)(value >> 24));
            WriteByte((byte)(value >> 16));
            WriteByte((byte)(value >> 8));
            WriteByte((byte)(value));
        }

        /// <summary>
        /// Write a 64 bit unsigned integer to the stream.
        /// </summary>
        /// <param name="value"></param>
        public void WriteUInt64(UInt64 value)
        {
            WriteByte((byte)(value >> 56));
            WriteByte((byte)(value >> 48));
            WriteByte((byte)(value >> 40));
            WriteByte((byte)(value >> 32));
            WriteByte((byte)(value >> 24));
            WriteByte((byte)(value >> 16));
            WriteByte((byte)(value >> 8));
            WriteByte((byte)(value));
        }
        #endregion
		
		
	}
}

