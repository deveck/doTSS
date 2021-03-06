/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


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

namespace Iaik.Tc.TPM.Library.Commands.StorageFunctions
{

	[TPMCommands(TPMCommandNames.TPM_CMD_CreateWrapKey)]
	public class TPM_CreateWrapKey : TPMCommandAuthorizable
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
		/// Contains the encoded usage auth of the new key
		/// </summary>
		private byte[] _usageAuth = null;
		
		/// <summary>
		/// Contains the encoded migration auth of the new key
		/// </summary>
		private byte[] _migrationAuth = null;
	
		/// <summary>
		/// Key specification of the key being created
		/// </summary>
		private TPMKeyCore _tpmKey = null;
	
		public override byte[] Digest 
		{
			get 
			{
				if(_digest == null)
				{
					_digest = new HashProvider().Hash(
						new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_CreateWrapKey),
						new HashByteDataProvider(_usageAuth),
						new HashByteDataProvider(_migrationAuth),
						new HashTPMBlobWritableDataProvider(_tpmKey));
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
					      new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_CreateWrapKey),
					      //3S
					      new HashStreamDataProvider(_responseBlob, offset + 4, _responseBlob.Length - offset - 4 - authHandleSize, false));
				}
				
				return _responseDigest;
			}
		}


		public override void Init (Parameters param, TPMProvider tpmProvider, TPMWrapper tpmWrapper)
		{
			base.Init (param, tpmProvider, tpmWrapper);
			
			
			_digest = null;
			
			
			_tpmKey = TPMKeyCore.Create (
			    CapabilityDataCore.TPMVersionCore.CreateVersion11(),
				_params.GetValueOf<TPMKeyUsage>("key_usage"),
				_params.GetValueOf<TPMKeyFlags>("key_flags"),
				TPMAuthDataUsage.TPM_AUTH_ALWAYS,
				TPMKeyParamsCore.Create (
					TPMAlgorithmId.TPM_ALG_RSA, 
                    _params.GetValueOf<TPMEncScheme>("enc_scheme"),
                    _params.GetValueOf<TPMSigScheme>("sig_scheme"),
					TPMRSAKeyParamsCore.Create (
						_params.GetValueOf<uint>("key_length"), 
						_params.GetValueOf<uint>("num_primes"),
						_params.GetValueOf<byte[]>("exponent"))
					),
				null, //Pubkey, use default (empty) pubkey
				null  //no encoded data
				);
		}


		protected override TPMCommandResponse InternalProcess ()
		{
			// Unencrypted authorization values, they need to be XOR-Encrypted with
			// XOR(auth, SHA-1(OSAP shared secret | session nonce))
			//
			// OSAP_shared_secret = HMAC(key=usage secret of key handle, nonce even osap | nonce odd osap)
			AuthHandle auth1OSAP = _commandAuthHelper.AssureOSAPSharedSecret(this, AuthSessionNum.Auth1);
			
			
			_usageAuth = _params.GetValueOf<byte[]> ("usage_auth");
			_migrationAuth = _params.GetValueOf<byte[]> ("migration_auth");
			byte[] xorKey = new HashProvider().Hash(
					new HashByteDataProvider(auth1OSAP.SharedSecret),
					new HashByteDataProvider(auth1OSAP.NonceEven));
			
			ByteHelper.XORBytes(_usageAuth, xorKey);
			ByteHelper.XORBytes(_migrationAuth, xorKey);
			
			//Load parent key if not loaded
			_keyManager.LoadKey(_params.GetValueOf<string>("parent"));
			
			TPMBlob requestBlob = new TPMBlob();
			requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_AUTH1_COMMAND, TPMOrdinals.TPM_ORD_CreateWrapKey);
			
			//parent key handle gets inserted later, it may be not available now
			requestBlob.WriteUInt32(0);
			requestBlob.Write(_usageAuth, 0, 20);
			requestBlob.Write(_migrationAuth, 0, 20);
			_tpmKey.WriteToTpmBlob(requestBlob);
			
			
			
			using(_keyManager.AcquireLock())
			{
				AuthorizeMe(requestBlob);
				requestBlob.SkipHeader();
				
				if(_params.GetValueOf<string>("parent") == KeyHandle.KEY_SRK)
					requestBlob.WriteUInt32((uint)TPMKeyHandles.TPM_KH_SRK);
				else
					requestBlob.WriteUInt32(_keyManager.IdentifierToHandle(_params.GetValueOf<string>("parent")).Handle);

				_responseBlob = TransmitMe(requestBlob);
			}
			
			CheckResponseAuthInfo();
		
			_responseBlob.SkipHeader();
			TPMKeyCore newKey = new TPMKeyCore(_responseBlob);
			_responseParameters = new Parameters();
			
			//Build and save the key identifier
			//The key identifier is the hex-string representation of the hash of the newly created key
			_responseParameters.AddPrimitiveType("key_identifier", 
				ByteHelper.ByteArrayToHexString(
					new HashProvider().Hash(
							new HashByteDataProvider(
								ByteHelper.SerializeToBytes(newKey)
							)
						),
					""));
					
			_responseParameters.AddPrimitiveType("key_data", ByteHelper.SerializeToBytes(newKey));
		
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_CreateWrapKey, _responseParameters);
		}
		
		
		public override HMACKeyInfo GetKeyInfo (AuthSessionNum authSessionNum)
		{
			if(authSessionNum != AuthSessionNum.Auth1)
				return null;
		
		
			string parentIdentifier = _params.GetValueOf<string>("parent");
			
			if(parentIdentifier == KeyHandle.KEY_SRK)
				return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.SrkSecret, new Parameters());
			else
			{
				Parameters parameters = new Parameters();
				parameters.AddPrimitiveType("identifier", parentIdentifier);
			
				return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyUsageSecret, parameters);
			}
			
		}


	
		public override bool SupportsAuthType (AuthHandle.AuthType authType)
		{
			return authType == AuthHandle.AuthType.OSAP;
		}


		public override string GetHandle (AuthSessionNum authSessionNum)
		{
			if(authSessionNum == AuthSessionNum.Auth1)
				return _params.GetValueOf<string>("parent");
			
			return null;
		}

	
		public override string ToString ()
		{
			return string.Format("[TPM_CreateWrapKey: HashCode '{0}']", GetHashCode());
		}

	}
}
