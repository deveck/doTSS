
using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.Common.KeyData
{


	[TypedStreamSerializable("TPM_STORE_PUBKEY")]
	public class TPMStorePubkey : AutoStreamSerializable, ITypedParameter
	{
		/// <summary>
		/// Stores the public key
		/// </summary>
		[SerializeMe(0)]
		protected byte[] _pubkey;		
		
		public byte[] Pubkey
		{
			get { return _pubkey;}
		}
			
		
		protected TPMStorePubkey ()
		{
		}
		
		public TPMStorePubkey (Stream src)
		{
			Read (src);
		}
		
		public override string ToString ()
		{
			return ByteHelper.ByteArrayToHexString (_pubkey);
		}

	}
}
