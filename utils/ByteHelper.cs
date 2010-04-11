
using System;
using System.Text;

namespace Iaik.Utils
{


	public static class ByteHelper
	{

		/// <summary>
		/// Converts a byte array to a space seperated hex string
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string ByteArrayToHexString (byte[] data)
		{
			StringBuilder returnVal = new StringBuilder ();
			
			foreach (byte b in data)
				returnVal.AppendFormat ("{0:X2} ", b);
			
			return returnVal.ToString ();
		}
		
		/// <summary>
		/// Compares the 2 byte arrays and returns if they contain the same data
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool CompareByteArrays (byte[] left, byte[] right)
		{
			if (left.Length != right.Length)
				return false;
			
			for (int i = 0; i < left.Length; i++)
			{
				if (left[i] != right[i])
					return false;
			}
			
			return true;
		}
		
		public static void ClearBytes (byte[] data)
		{
			Array.Clear (data, 0, data.Length);
		}
		
	}
}
