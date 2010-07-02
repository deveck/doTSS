using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto;
using Iaik.Utils.Hash;
using System.Security.Cryptography;
using Iaik.Tc.TPM.Library.Common;
using Org.BouncyCastle.Crypto.Digests;

namespace Iaik.Tc.TPM.Context
{
    /// <summary>
    /// Implementation of the <see>ISigner</see> interface for signing operations on the TPM.
    /// </summary>
    /// <remarks>
    /// This implementation does as much as possible on the client side and only transmits the neccessary data to the server,
    /// therefore it needs to take care of the signature format on the client side
    /// </remarks>
    public class TPMRSASHA1Signer : ISigner
    {
        /// <summary>
        /// The Keyhandle this signer belongs to
        /// </summary>
        private ClientKeyHandle _keyHandle;

        /// <summary>
        /// The session this signer belongs to 
        /// </summary>
        private TPMSession _session;

		/// <summary>
		/// The used hash algorithm 
		/// </summary>
		private IDigest _hashAlgorithm;

		private ISigner _signatureVerifier;
		
		private bool _forSigning;
		
		
		
		
        public TPMRSASHA1Signer(TPMSession session, ClientKeyHandle keyHandle)
        {
            _session = session;
            _keyHandle = keyHandle;
			
			if(_keyHandle.KeyInfo.AlgorithmParams.AlgorithmId != TPMAlgorithmId.TPM_ALG_RSA ||
			   _keyHandle.KeyInfo.AlgorithmParams.SigScheme != TPMSigScheme.TPM_SS_RSASSAPKCS1v15_SHA1)
			{
				throw new ArgumentException("Invalid key handle for TPMRSASHA1Signer");
			}
            
        }

        #region ISigner Members

        public string AlgorithmName
        {
            get { return "SHA1"; }
        }

        public void BlockUpdate(byte[] input, int inOff, int length)
        {
			if(_forSigning)			
				_hashAlgorithm.BlockUpdate(input, inOff, length);
			else
				_signatureVerifier.BlockUpdate(input, inOff, length);
        }

        public byte[] GenerateSignature()
        {
            if(!_forSigning)
				throw new ArgumentException("TPMRSASHA1Signer is not initialized for signatur generation");

			byte[] hash = new byte[_hashAlgorithm.GetDigestSize()];
			_hashAlgorithm.DoFinal(hash, 0);
			
			return _keyHandle.SignWithProperFormat(hash);
			
        }

        public void Init(bool forSigning, ICipherParameters parameters)
        {
			_forSigning = forSigning;
			
			if(_forSigning)
				_hashAlgorithm = new Sha1Digest();
			else
				_signatureVerifier = _keyHandle.PublicKey.CreateSignatureVerificator();
        }

        public void Reset()
        {
            Init(_forSigning, null);
        }

        public void Update(byte input)
        {
            BlockUpdate(new byte[]{input}, 0, 1);
        }

        public bool VerifySignature(byte[] signature)
        {
            if(_forSigning)
				throw new ArgumentException("TPMRSASHA1Signer is not initialized for verification");
			
			return _signatureVerifier.VerifySignature(signature);
        }

        #endregion
    }
}
