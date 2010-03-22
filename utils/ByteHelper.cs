
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
		
	}
}
