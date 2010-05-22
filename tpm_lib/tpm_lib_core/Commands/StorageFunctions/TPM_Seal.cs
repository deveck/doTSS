// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Tc.TPM.Library.KeyDataCore;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Utils.Hash;
using Iaik.Utils;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Hash;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;
using Iaik.Tc.TPM.Library.PCRDataCore;
using Iaik.Tc.TPM.Library.Common.PCRData;
using Iaik.Tc.TPM.Library.Storage;

namespace Iaik.Tc.TPM.Library.Commands.StorageFunctions
{

	[TPMCommands(TPMCommandNames.TPM_CMD_Seal)]
	public class TPM_Seal : TPMCommandAuthorizable
	{
		/// <summary>
		/// The outgoing digest for this command
		/// </summary>
		private byte[] _digest = null;	
	
		/// <summary>
		/// Digest of the tpm response
		/// </summary>
		private byte[] _responseDigest = null;
	
		/// <summary>
		/// Contains the encrypted authdata for the sealed data
		/// </summary>
		private byte[] _encAuth = null;
		
		/// <summary>
		/// contains the PCR constraints for the data to be sealed
		/// </summary>
		private TPMPCRInfoCore _pcrInfo = null;

		/// <summary>
		/// Contains the data to be sealed
		/// </summary>
		private byte[] _inData = null;
		
		public override byte[] Digest 
		{
			get 
			{
				if(_digest == null)
				{
					using(TPMBlob tempBlob = new TPMBlob())
					{
						TPMBlobWriteableHelper.WriteITPMBlobWritableWithUIntSize(tempBlob, _pcrInfo);
						
						_digest = new HashProvider().Hash(
							new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_Seal),
							new HashByteDataProvider(_encAuth),
							new HashStreamDataProvider(tempBlob, 0, null, false),
							new HashPrimitiveDataProvider((uint)_inData.Length),
							new HashByteDataProvider(_inData));
					}
				}
				
				return _digest;
			}
		}

	
		public override byte[] ResponseDigest 
		{
			get 
			{
				if(_responseDigest == null)
				{
					HashProvider hasher = new HashProvider();
					
					int offset = 2+4; //tag + paramsize
					
					int authHandleSize = ResponseAuthHandleInfoCore.ReadAuthHandleInfos(this, _responseBlob).Length *
						ResponseAuthHandleInfoCore.SINGLE_AUTH_HANDLE_SIZE;
					
					_responseDigest = hasher.Hash(
					      //1S
					      new HashStreamDataProvider(_responseBlob, offset, 4, false),
					      //2S
					      new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_Seal),
					      //3S
					      new HashStreamDataProvider(_responseBlob, offset + 4, _responseBlob.Length - offset - 4 - authHandleSize, false));
				}
				
				return _responseDigest;
			}
		}


		public override void Init (Parameters param, TPMProvider tpmProvider)
		{
			base.Init (param, tpmProvider);
			
			
			_digest = null;
			_responseDigest = null;
			_inData = param.GetValueOf<byte[]>("in_data");
			_pcrInfo = new TPMPCRInfoCore(new TPMPCRSelectionCore(param.GetValueOf<TPMPCRSelection>("pcr_selection")));
			
			
			
		}


		protected override TPMCommandResponse InternalProcess ()
		{
			// Unencrypted authorization values, they need to be XOR-Encrypted with
			// XOR(auth, SHA-1(OSAP shared secret | session nonce))
			//
			// OSAP_shared_secret = HMAC(key=usage secret of key handle, nonce even osap | nonce odd osap)
			AuthHandle auth1OSAP = _commandAuthHelper.AssureOSAPSharedSecret(this, AuthSessionNum.Auth1);			
			
			_encAuth = _params.GetValueOf<byte[]> ("data_auth");

			byte[] xorKey = new HashProvider().Hash(
					new HashByteDataProvider(auth1OSAP.SharedSecret),
					new HashByteDataProvider(auth1OSAP.NonceEven));
			
			ByteHelper.XORBytes(_encAuth, xorKey);
			
			//Load parent key if not loaded
			_keyManager.LoadKey(_params.GetValueOf<string>("key"));
			
			TPMBlob requestBlob = new TPMBlob();
			requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_AUTH1_COMMAND, TPMOrdinals.TPM_ORD_Seal);
			
			//key handle gets inserted later, it may be not available now
			requestBlob.WriteUInt32(0);
			requestBlob.Write(_encAuth, 0, 20);
			TPMBlobWriteableHelper.WriteITPMBlobWritableWithUIntSize(requestBlob, _pcrInfo);
			requestBlob.WriteUInt32((uint)_inData.Length);
			requestBlob.Write(_inData, 0, _inData.Length);
						
			AuthorizeMe(requestBlob);
			
			using(_keyManager.AcquireLock())
			{
				requestBlob.SkipHeader();
				requestBlob.WriteUInt32(_keyManager.IdentifierToHandle(_params.GetValueOf<string>("key")).Handle);					
				_responseBlob = TransmitMe(requestBlob);
			}
			
			CheckResponseAuthInfo();
		
			_responseBlob.SkipHeader();
			
			TPMStoredDataCore sealedData = TPMStoredDataCore.CreateFromTPMBlob(_responseBlob);
			
			Parameters responseParams = new Parameters();
			responseParams.AddPrimitiveType("data", ByteHelper.SerializeToBytes(sealedData)); 
		
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_Seal, responseParams);
		}
		
		
		public override HMACKeyInfo GetKeyInfo (AuthSessionNum authSessionNum)
		{
			if(authSessionNum != AuthSessionNum.Auth1)
				return null;
		
		
			string keyIdentifier = _params.GetValueOf<string>("key");
			
			Parameters parameters = new Parameters();
			parameters.AddPrimitiveType("identifier", keyIdentifier);
			
			return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyUsageSecret, parameters);
		}


	
		public override bool SupportsAuthType (AuthHandle.AuthType authType)
		{
			return authType == AuthHandle.AuthType.OSAP;
		}


		public override string GetHandle (AuthSessionNum authSessionNum)
		{
			if(authSessionNum == AuthSessionNum.Auth1)
				return _params.GetValueOf<string>("key");
			
			return null;
		}

	
	}
}
