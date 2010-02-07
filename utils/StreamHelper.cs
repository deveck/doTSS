//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
using System;
using System.IO;
using System.Text;

namespace Iaik.Utils
{
	/// <summary>
	/// Provides methods of writing/reading simple data types to/from streams
	/// </summary>
	public static class StreamHelper
	{
		public static void WriteUInt16(ushort value, Stream sink)
		{
			WriteBytes(BitConverter.GetBytes(value), sink);
		}
		
		public static ushort ReadUInt16(Stream src)
		{
			return BitConverter.ToUInt16(ReadBytes(2, src), 0);
		}
		
		public static void WriteInt32(Int32 value, Stream sink)
		{
			WriteBytes(BitConverter.GetBytes(value), sink);
		}
		
		public static int ReadInt32(Stream src)
		{
			return BitConverter.ToInt32(ReadBytes(4, src), 0);
		}
		
		public static void WriteString(String value, Stream sink)
		{
			if(value == null)
				WriteInt32(-1, sink);
			WriteInt32(Encoding.UTF8.GetByteCount(value), sink);
			WriteBytes(Encoding.UTF8.GetBytes(value), sink);
		}
		
		public static string ReadString(Stream src)
		{
			int length = ReadInt32(src);
			if(length == -1)
				return null;
			return Encoding.UTF8.GetString(ReadBytes(length, src));
		}

		public static void WriteBytesSafe(byte[] data, Stream sink)
		{
			if(data == null)
				WriteInt32(-1, sink);
			else
			{
				WriteInt32(data.Length, sink);
				WriteBytes(data, sink);
			}
		}
		
		public static byte[] ReadBytesSafe(Stream src)
		{
			int length = ReadInt32(src);
			if(length == -1)
				return null;
			else
				return ReadBytes(length, src);
		}
		
		private static void WriteBytes(byte[] buf, Stream sink)
		{
			sink.Write(buf, 0, buf.Length);
		}
			
		
		private static byte[] ReadBytes(int length, Stream src)
		{
			byte[] buf = new byte[length];
			int read = src.Read(buf, 0, length);
			return buf;
		}
		

		public static void WriteBool(bool isResponse, Stream sink)
		{
			sink.WriteByte(isResponse?(byte)1:(byte)0);
		}
		
		public static bool ReadBool (Stream src)
		{
			return src.ReadByte() != 0;
		}
	}
}
