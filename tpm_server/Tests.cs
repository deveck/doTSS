// ///
// ///
// /// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// /// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils;
using System.IO;

namespace Iaik.Tc.Tpm
{


	public class Tests
	{

		public static void TestIt()
		{
			Tests t = new Tests();
			t.TestByteStream();
		}
		
		public bool TestByteStream()
		{
			byte[] data = new byte[1024];
			for(int i = 0; i<1024; i++)
				data[i] = (byte)(i%256);
			
			ByteStream bStream = new ByteStream(data);
			
			byte[] buffer = new byte[512];
			bStream.Read(buffer, 0, 512);
			for(int bufferIndex = 0; bufferIndex<buffer.Length; bufferIndex++)
			{
				if(buffer[bufferIndex] != bufferIndex % 256)
					throw new Exception("Expecting " + (bufferIndex).ToString());
			}
			
			bStream.Seek(0, SeekOrigin.Begin);
			if(bStream.ReadByte() != 0)
				throw new Exception("Expecting 0");
			
			if(bStream.ReadByte() != 1)
				throw new Exception("Expecting 1");
			
			bStream.Seek(510, SeekOrigin.Current);
			bStream.Read(buffer, 0, 512);
			for(int i = 0; i<buffer.Length; i++)
			{
				if(buffer[i] != (i+512)%256)
					throw new Exception("Expecting " + (i+512).ToString());
			}
			return true;
		}
	}
}
