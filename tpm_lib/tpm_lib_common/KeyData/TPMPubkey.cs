
using System;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Utils;
using System.IO;
using System.Security.Cryptography;

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
		protected TPMStorePubkey _publicKey;
		
		/// <summary>
		/// The public key
		/// </summary>
		public TPMStorePubkey PublicKey
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
		

		/// <summary>
		/// Creates an asymmetric algorithm instance (RSA) with the public key of this instance,
		/// ready for encryption
		/// </summary>
		/// <returns></returns>
		public RSA CreateRSAEncrypter ()
		{
			if (_keyParams.AlgorithmId == TPMAlgorithmId.TPM_ALG_RSA) 
			{
				RSA rsaInstance = new RSACryptoServiceProvider ();
				RSAParameters parameters = new RSAParameters ();
				parameters.Modulus = _publicKey.Pubkey;
				parameters.Exponent = ((TPMRSAKeyParams)_keyParams.Params).GetExponent();
				
//				Console.WriteLine ("P: {0}\nQ: {1}\nD: {2}\nDP: {3}\nDQ: {4}\nExpo: {5}\nInvQ: {6}\nMod: {7}", 
//				ByteHelper.ByteArrayToHexString (parameters.P), 
//				ByteHelper.ByteArrayToHexString (parameters.Q), 
//				ByteHelper.ByteArrayToHexString (parameters.D), 
//				ByteHelper.ByteArrayToHexString (parameters.DP), 
//				ByteHelper.ByteArrayToHexString (parameters.DQ), 
//				ByteHelper.ByteArrayToHexString (parameters.Exponent), 
//				ByteHelper.ByteArrayToHexString (parameters.InverseQ), 
//				ByteHelper.ByteArrayToHexString (parameters.Modulus));
				
				rsaInstance.ImportParameters (parameters);
				return rsaInstance;
			} 
			else
			 throw new NotSupportedException (string.Format ("Algorithm '{0}' is not supported", _keyParams.AlgorithmId));
		}

		
		public override string ToString ()
		{
			return string.Format ("KeyParams:\n{0}\n\nPublicKey:\n{1}", 
				StringHelper.IndentPerLine (KeyParams.ToString (), "   "), 
				_publicKey.ToString());
		}

	}
}
