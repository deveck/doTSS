
using System;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Lowlevel.Data;
using System.IO;

namespace Iaik.Tc.TPM.Library.KeyDataCore
{

	[TypedStreamSerializable("TPM_STORE_PUBKEY")]
	public class TPMStorePubkeyCore : TPMStorePubkey, ITPMBlobReadable, ITPMBlobWritable
	{
		/// <summary>
		/// Creates an empty pubkey object with key length = 0
		/// </summary>
		/// <returns></returns>
		public static TPMStorePubkeyCore CreateEmptyPubkey ()
		{
			TPMStorePubkeyCore pubkey = new TPMStorePubkeyCore ();
			pubkey._pubkey = new Byte[0];
			return pubkey;
		}
		
		public static TPMStorePubkeyCore CreateFromTpmBlob(TPMBlob blob)
		{
			TPMStorePubkeyCore pubkey = new TPMStorePubkeyCore();
			pubkey.ReadFromTpmBlob(blob);
			return pubkey;
		}
		
		private TPMStorePubkeyCore()
		{
		}
		
		public TPMStorePubkeyCore (Stream src)
		{
			Read(src);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			uint keyLength = blob.ReadUInt32 ();
			_pubkey = blob.ReadBytes ((int)keyLength);			
		}
		
		#endregion
		#region ITPMBlobWritable implementation
		public void WriteToTpmBlob (TPMBlob blob)
		{
			blob.WriteUInt32 ((uint)_pubkey.Length);
			blob.Write (_pubkey, 0, _pubkey.Length);
		}
		
		#endregion
	}
}
