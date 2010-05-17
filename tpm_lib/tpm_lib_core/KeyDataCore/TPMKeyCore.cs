
using System;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Commands;
using Iaik.Tc.TPM.Library.Common;
using System.IO;

namespace Iaik.Tc.TPM.Library.KeyDataCore
{


	[TypedStreamSerializable("TPM_Key")]
	public class TPMKeyCore : TPMKey, ITPMBlobReadable, ITPMBlobWritable
	{
		public static TPMKeyCore CreateFromStream(Stream sink)
		{
			TPMKeyCore tpmKey = new TPMKeyCore();
			tpmKey.Read(sink);
			return tpmKey;
		}
	
		public static TPMKeyCore Create (CapabilityDataCore.TPMVersionCore version, TPMKeyUsage keyUsage, TPMKeyFlags keyFlags, 
			TPMAuthDataUsage authDataUsage, TPMKeyParamsCore algorithmParams, TPMStorePubkeyCore pubkey, byte[] encdata)
		{
			TPMKeyCore key = new TPMKeyCore ();
			
			
			key._keyUsage = keyUsage;
			key._keyFlags = keyFlags;
			key._authDataUsage = authDataUsage;
			key._algorithmParams = algorithmParams;
			
			
			if (version == null)
				key._version = CapabilityDataCore.TPMVersionCore.CreateVersion12 ();
			else
				key._version = version;
			
			if (pubkey == null)
				key._pubKey = TPMStorePubkeyCore.CreateEmptyPubkey ();
			else
				key._pubKey = pubkey;
			
			if (encdata == null)
				key._encData = new byte[0];
			else
				key._encData = encdata;
			
			return key;
			
		}
		
		private TPMKeyCore()
		{
		}
		
		public TPMKeyCore (TPMBlob blob)
		{
			ReadFromTpmBlob (blob);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_version = CapabilityDataCore.TPMVersionCore.CreateFromTPMBlob (blob);
			_keyUsage = (TPMKeyUsage)blob.ReadUInt16 ();
			_keyFlags = (TPMKeyFlags)blob.ReadUInt32 ();
			_authDataUsage = (TPMAuthDataUsage)blob.ReadByte ();
			_algorithmParams = new TPMKeyParamsCore (blob);
			
			uint pcrInfoSize = blob.ReadUInt32 ();
			/*byte[] pcrInfo =*/ blob.ReadBytes ((int)pcrInfoSize);
			
			_pubKey = TPMStorePubkeyCore.CreateFromTpmBlob (blob);
			
			uint encDataSize = blob.ReadUInt32 ();
			_encData = blob.ReadBytes ((int)encDataSize);
		}
		
		#endregion
		#region ITPMBlobWritable implementation
		public void WriteToTpmBlob (TPMBlob blob)
		{
			((ITPMBlobWritable)_version).WriteToTpmBlob (blob);
			blob.WriteUInt16 ((ushort)_keyUsage);
			blob.WriteUInt32 ((uint)_keyFlags);
			blob.WriteByte ((byte)_authDataUsage);
			((ITPMBlobWritable)_algorithmParams).WriteToTpmBlob (blob);
			
			//TODO: PCR info size
			blob.WriteUInt32 (0);
			
			((ITPMBlobWritable)_pubKey).WriteToTpmBlob (blob);
			
			blob.WriteUInt32 ((uint)_encData.Length);
			blob.Write (_encData, 0, _encData.Length);
		}
		
		#endregion

	}
}
