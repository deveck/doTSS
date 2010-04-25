using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Text;

namespace Iaik.Utils.Hash
{

	/// <summary>
	/// Does not support stream serialization
	/// </summary>
	public class HashSecureStringDataProvider : HashDataProvider
	{
		
		/// <summary>
		/// Provides the secure string to hash
		/// </summary>
		private SecureString _src;
		
		/// <summary>
		/// Current location in _src
		/// </summary>
		private int _currentIndex = 0;
		
		public HashSecureStringDataProvider (SecureString src)
		{
			_src = src;
		}
		

		public override int NextBytes (byte[] buffer)
		{
			IntPtr secureStringPtr = Marshal.SecureStringToBSTR (_src);
			
			try
			{
				unsafe 
				{
					char* secureStringP = (char*)secureStringPtr;
					int read = 0;
					int charsRead = 0;
					
					for (int i = _currentIndex; i < buffer.Length; i++)
					{
						if (secureStringP[i] == 0)
							break;
						else
						{
							int byteCount = Encoding.UTF8.GetByteCount (new char[] { secureStringP[i] });
							
							//Check if the buffer is large enough to hold the bytes of the current car
							if (read + byteCount > buffer.Length)
								break;
							
							int charBytes = Encoding.UTF8.GetBytes (new char[] { secureStringP[i] }, 0, 1, buffer, read);
							read += charBytes;
							charsRead++;
						}
					}
					
					_currentIndex += charsRead;
					return read;
				
				}
			}
			finally
			{
				Marshal.ZeroFreeBSTR (secureStringPtr);
			}
		}

	}
}
