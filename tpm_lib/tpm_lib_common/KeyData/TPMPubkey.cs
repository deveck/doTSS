
using System;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Utils;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System.Text;
using Iaik.Utils.Hash;
using Org.BouncyCastle.Crypto.Signers;

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
		public IAsymmetricBlockCipher CreateRSAEncrypter ()
		{
			if (_keyParams.AlgorithmId == TPMAlgorithmId.TPM_ALG_RSA && _keyParams.EncScheme == TPMEncScheme.TPM_ES_RSAESOAEP_SHA1_MGF1) 
			{
				IAsymmetricBlockCipher cipher = new OaepEncoding(new RsaEngine(), new Sha1Digest(), Encoding.ASCII.GetBytes("TCPA"));
				
				RsaKeyParameters parameters = 
					new RsaKeyParameters( false,
					                     new BigInteger(1, _publicKey.Pubkey),
					                     new BigInteger(1, ((TPMRSAKeyParams)_keyParams.Params).GetExponent()));
				                                             
				cipher.Init(true, parameters);
				
				return cipher;                                       
			} 
			else if (_keyParams.AlgorithmId == TPMAlgorithmId.TPM_ALG_RSA && _keyParams.EncScheme == TPMEncScheme.TPM_ES_RSAESPKCSv15) 
			{
				IAsymmetricBlockCipher cipher = new Pkcs1Encoding(new RsaEngine());
				
				RsaKeyParameters parameters = 
					new RsaKeyParameters( false,
					                     new BigInteger(1, _publicKey.Pubkey),
					                     new BigInteger(1, ((TPMRSAKeyParams)_keyParams.Params).GetExponent()));
				                                             
				cipher.Init(true, parameters);
				
				return cipher;       
			}
            else
                throw new NotSupportedException(string.Format("Algorithm '{0}' with '{1}' is not supported", _keyParams.AlgorithmId, _keyParams.EncScheme));
		}


        /// <summary>
        /// Creates a signature verificator (signature (created on tpm) -> message digest) with the current public key and
        /// for the current algorithm (RSA)
        /// </summary>
        /// <returns></returns>
        public ISigner CreateSignatureVerificator()
        {
            if (_keyParams.AlgorithmId == TPMAlgorithmId.TPM_ALG_RSA && _keyParams.SigScheme == TPMSigScheme.TPM_SS_RSASSAPKCS1v15_SHA1)
            {
                ISigner signer = new RsaDigestSigner(new Sha1Digest());

                RsaKeyParameters parameters =
                    new RsaKeyParameters(false,
                                         new BigInteger(1, _publicKey.Pubkey),
                                         new BigInteger(1, ((TPMRSAKeyParams)_keyParams.Params).GetExponent()));

                signer.Init(false, parameters);

                return signer;    
            }
            else
                throw new NotSupportedException(string.Format("SignatureVerificator: Algorithm '{0}' with '{1}' is not supported", _keyParams.AlgorithmId, _keyParams.SigScheme));

        }
		
		public override string ToString ()
		{
			return string.Format ("KeyParams:\n{0}\n\nPublicKey:\n{1}", 
				StringHelper.IndentPerLine (KeyParams.ToString (), "   "), 
				_publicKey.ToString());
		}

	}
}
