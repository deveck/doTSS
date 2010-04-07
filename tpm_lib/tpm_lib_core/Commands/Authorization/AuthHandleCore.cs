
using System;
using Iaik.Tc.TPM.Subsystems.TPMSubsystem.Authorization;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Utils.Serialization;

namespace Iaik.Tc.TPM.Library.Commands
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
		}
		
		#endregion

		
	}
}
