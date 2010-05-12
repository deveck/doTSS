
using System;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;

namespace Iaik.Tc.TPM.Library.HandlesCore.Authorization
{


	[TypedStreamSerializable("AuthHandle")]
	public class AuthHandleCore : AuthHandle, ITPMBlobReadable
	{
		
		public AuthHandleCore(AuthType authType, TPMBlob blob)
		{
			_authType = authType;
			ReadFromTpmBlob(blob);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_authHandle = blob.ReadUInt32();
			
			_nonceEven = new byte[20];
			blob.Read(_nonceEven, 0, _nonceEven.Length);
			
			if(_authType == AuthHandle.AuthType.OSAP)
			{
				_nonceEvenOSAP = new byte[20];
				blob.Read(_nonceEvenOSAP, 0, _nonceEvenOSAP.Length);
			}
		}
		
		#endregion

		
	}
}
