// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using System.Collections.Generic;
using System.IO;

namespace Iaik.Tc.TPM.Library.HandlesCore.Authorization
{

	/// <summary>
	/// Reads the response authorization handle information 
	/// from a tpm response
	/// </summary>
	public class ResponseAuthHandleInfoCore: ResponseAuthHandleInfo
	{
		public const int SINGLE_AUTH_HANDLE_SIZE = 20 //nonceEven
				+ 1 //continue auth session
				+ 20 //TPM_Authdata
				;
		
		#region ITPMBlobReadable implementation
		public static ResponseAuthHandleInfo[] ReadAuthHandleInfos (IAuthorizableCommand cmd, TPMBlob blob)
		{
			long currentIndex = blob.Length;
			
			List<ResponseAuthHandleInfo> responseAuthHandles = new List<ResponseAuthHandleInfo>();
			
			foreach(AuthSessionNum authSession in new AuthSessionNum[]{AuthSessionNum.Auth2, AuthSessionNum.Auth1})
			{
				HMACKeyInfo keyInfo = cmd.GetKeyInfo(authSession);
				
				if(keyInfo != null)
				{
					currentIndex -= SINGLE_AUTH_HANDLE_SIZE;
					responseAuthHandles.Add(new ResponseAuthHandleInfoCore(blob, currentIndex));
				}
			}
			
			responseAuthHandles.Reverse();
			return responseAuthHandles.ToArray();
		}
		
		#endregion
		
		
		public ResponseAuthHandleInfoCore (TPMBlob blob, long startIndex)
		{
			blob.Seek(startIndex, SeekOrigin.Begin);
			_nonceEven = blob.ReadBytes(20);
			_continueAuthSession = blob.ReadBool();
			_tpmAuthData = blob.ReadBytes(20);
		}
		
		
	}
}
