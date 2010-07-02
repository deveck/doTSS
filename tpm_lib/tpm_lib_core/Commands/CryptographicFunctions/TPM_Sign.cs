using System;
using System.Collections.Generic;
using System.Text;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Tc.TPM.Library.KeyDataCore;

namespace Iaik.Tc.TPM.Library.Commands.CryptographicFunctions
{

    /// <summary>
    /// The Sign command signs data and returns the resulting digital signature
    /// </summary>
    [TPMCommands(TPMCommandNames.TPM_CMD_Sign)]
    public class TPM_Sign : TPMCommandAuthorizable
    {
        /// <summary>
        /// Digest of the command
        /// </summary>
        protected byte[] _digest = null;

        /// <summary>
        /// Dogest of the response
        /// </summary>
        protected byte[] _responseDigest = null;
        

        public override byte[] Digest
        {
            get
            {
                if (_digest == null)
                {
                    byte[] areaToSign = _params.GetValueOf<byte[]>("areaToSign");

                    _digest = new HashProvider().Hash(
                        new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_Sign),
                        new HashPrimitiveDataProvider((uint)areaToSign.Length),
                        new HashByteDataProvider(areaToSign));
                }

                return _digest;
            }
        }

        public override byte[] ResponseDigest
        {
            get { throw new NotImplementedException(); }
        }

        public override bool SupportsAuthType(AuthHandle.AuthType authType)
        {
            return authType == AuthHandle.AuthType.OIAP;
        }

        public override HMACKeyInfo GetKeyInfo(AuthSessionNum authSessionNum)
        {
            if (authSessionNum == AuthSessionNum.Auth1)
            {
                Parameters parameters = new Parameters();
                parameters.AddPrimitiveType("identifier", _params.GetValueOf<string>("key"));
                return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyUsageSecret, parameters);
            }
            else 
                return null;
        }

        public override string GetHandle(AuthSessionNum authSessionNum)
        {
            if (authSessionNum == AuthSessionNum.Auth1)
                return _params.GetValueOf<string>("key");
            else
                return null;
        }

        public override void Init(Parameters param, TPMProvider tpmProvider, TPMWrapper tpmWrapper)
        {
            base.Init(param, tpmProvider, tpmWrapper);

            _digest = null;
            _responseDigest = null;
            _responseBlob = null;
        }
        protected override TPMCommandResponse InternalProcess()
        {
            string key = _params.GetValueOf<string>("key");
            _keyManager.LoadKey(key);

            TPMKey keyInfo = TPMKeyCore.CreateFromBytes(_keyManager.GetKeyBlob(key));

            if(keyInfo == null)
                throw new ArgumentException(string.Format("TPM_Sign could not retrieve keyinfo for key '{0}'", key));

            byte[] areaToSign = null;

            if(keyInfo.AlgorithmParams.SigScheme == TPMSigScheme.TPM_SS_RSASSAPKCS1v15_SHA1)
            {
                //Client has hopefully put data in the right format ready for the tpm to process
                if(_params.IsDefined<byte[]>("areaToSign"))
                    areaToSign = _params.GetValueOf<byte[]>("areaToSign");
                //Client just sends data, tpm lib cares about the right, signature dependent, processing
                else if(_params.IsDefined<byte[]>("data"))
                {
                    byte[] data = _params.GetValueOf<byte[]>("data");

                    areaToSign = new HashProvider().Hash(new HashByteDataProvider(data));
                }

                if(areaToSign.Length != 20)
                {
                    throw new ArgumentException(string.Format("Sig scheme '{0}' expects an area to sign with length 20!", keyInfo.AlgorithmParams.SigScheme));
                }
            }
            else
                throw new ArgumentException(string.Format("TPM_Sign has not implemented signature scheme '{0}' for algorithm '{1}'", keyInfo.AlgorithmParams.SigScheme, keyInfo.AlgorithmParams.AlgorithmId));


            TPMBlob requestBlob = new TPMBlob();
            requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_AUTH1_COMMAND, TPMOrdinals.TPM_ORD_Sign);

            //key handle gets inserted later, it may be not available now
            requestBlob.WriteUInt32(0);
            requestBlob.WriteUInt32((uint)areaToSign.Length);
            requestBlob.Write(areaToSign, 0, areaToSign.Length);

            AuthorizeMe(requestBlob);

            using (_keyManager.AcquireLock())
            {
                requestBlob.SkipHeader();
                requestBlob.WriteUInt32(_keyManager.IdentifierToHandle(key).Handle);

                _responseBlob = TransmitMe(requestBlob);
            }

            CheckResponseAuthInfo();

            _responseBlob.SkipHeader();

            uint sigSize = _responseBlob.ReadUInt32();
            byte[] signature = _responseBlob.ReadBytes((int)sigSize);

            Parameters responseParams = new Parameters();
            responseParams.AddPrimitiveType("sig", signature);
            return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_Sign, responseParams);
        }
    }
}
