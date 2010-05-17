
using System;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Utils;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;
using System.IO;

namespace Iaik.Tc.TPM.Library.KeyDataCore
{

	[TypedStreamSerializable(TPMPubkey.PARAM_TPM_PUBKEY)]
	public class TPMPubkeyCore : TPMPubkey, ITPMBlobReadable
	{
		public static TPMPubkeyCore CreateFromTPMBlob(TPMBlob src)
		{
			TPMPubkeyCore pubkey = new TPMPubkeyCore();
			pubkey.ReadFromTpmBlob(src);
			return pubkey;
		}
	
		private TPMPubkeyCore()
		{
		}
	
		public TPMPubkeyCore (Stream src)
		{
			Read(src);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_keyParams = TPMKeyParamsCore.CreateFromTPMBlob(blob);
			_publicKey = TPMStorePubkeyCore.CreateFromTpmBlob (blob);
		}
		
		#endregion
	

	}
}
