using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto;
using Iaik.Utils.Hash;

namespace Iaik.Tc.TPM.Context
{
    /// <summary>
    /// Implementation of the <see>ISigner</see> interface for signing operations on the TPM.
    /// </summary>
    /// <remarks>
    /// This implementation does as much as possible on the client side and only transmits the neccessary data to the server,
    /// therefore it needs to take care of the signature format on the client side
    /// </remarks>
    public class TPMSigner : ISigner
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
        /// The hasher used to hash
        /// </summary>
        private HashProvider _hashProvider;

        public TPMSigner(TPMSession session, ClientKeyHandle keyHandle)
            :this(session, keyHandle, "SHA1")
        {
            
        }

        public TPMSigner(TPMSession session, ClientKeyHandle keyHandle, string hashAlgorithm)
        {
            _session = session;
            _keyHandle = keyHandle;
            _hashProvider = hashProvider;
        }

        #region ISigner Members

        public string AlgorithmName
        {
            get { return _hashProvider.HashAlgorithmName; }
        }

        public void BlockUpdate(byte[] input, int inOff, int length)
        {
            throw new NotImplementedException();
        }

        public byte[] GenerateSignature()
        {
            throw new NotImplementedException();
        }

        public void Init(bool forSigning, ICipherParameters parameters)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Update(byte input)
        {
            throw new NotImplementedException();
        }

        public bool VerifySignature(byte[] signature)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
