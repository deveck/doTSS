
using System;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Utils;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common
{

	[TypedStreamSerializable(TPMPubkey.PARAM_TPM_PUBKEY)]
	public class TPMPubkey : AutoStreamSerializable, ITypedParameter
	{
		public const string PARAM_TPM_PUBKEY = "TPM_PUBKEY";
		
		[SerializeMe(0)]
		protected TPMKeyParams _keyParams;
		
		/// <summary>
		/// Informations regarding this key
		/// </summary>
		public TPMKeyParams KeyParams
		{
			get { return _keyParams; }
		}
		
		[SerializeMe(1)]
		protected byte[] _publicKey;
		
		/// <summary>
		/// The public key
		/// </summary>
		public byte[] PublicKey
		{
			get { return _publicKey; }
		}
		
		
		protected TPMPubkey ()
		{
		}
		
		public TPMPubkey (Stream src)
		{
			Read (src);
		}
		
		
		public override string ToString ()
		{
			return string.Format ("KeyParams:\n{0}\n\nPublicKey:\n{1}", 
				StringHelper.IndentPerLine (KeyParams.ToString (), "   "), 
				ByteHelper.ByteArrayToHexString(_publicKey));
		}

	}
}
