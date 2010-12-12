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
using Iaik.Tc.TPM.Library.PCRDataCore;
using Iaik.Tc.TPM.Library.Common.PCRData;
using Iaik.Tc.TPM.Library.Storage;

namespace Iaik.Tc.TPM.Library.Commands.StorageFunctions
{

	[TPMCommands(TPMCommandNames.TPM_CMD_Unseal)]
	public class TPM_Unseal : TPMCommandAuthorizable
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
		/// Contains the data to be unsealed
		/// </summary>
		private TPMStoredDataCore _inData = null;
		
		public override byte[] Digest 
		{
			get 
			{
				if(_digest == null)
				{
					_digest = new HashProvider().Hash(
						new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_Unseal),
						new HashTPMBlobWritableDataProvider(_inData));
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
					      new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_Unseal),
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
			_responseDigest = null;
			_inData = TPMStoredDataCore.CreateFromBytes(param.GetValueOf<byte[]>("in_data"));
			
		}


		protected override TPMCommandResponse InternalProcess ()
		{
			//Load parent key if not loaded
			_keyManager.LoadKey(_params.GetValueOf<string>("key"));
			
			TPMBlob requestBlob = new TPMBlob();
			requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_AUTH2_COMMAND, TPMOrdinals.TPM_ORD_Unseal);
			
			//key handle gets inserted later, it may be not available now
			requestBlob.WriteUInt32(0);
			_inData.WriteToTpmBlob(requestBlob);
						
			AuthorizeMe(requestBlob);
			
			using(_keyManager.AcquireLock())
			{
				requestBlob.SkipHeader();
				requestBlob.WriteUInt32(_keyManager.IdentifierToHandle(_params.GetValueOf<string>("key")).Handle);					
				_responseBlob = TransmitMe(requestBlob);
			}
			
			CheckResponseAuthInfo();
		
			_responseBlob.SkipHeader();
			
			uint dataSize = _responseBlob.ReadUInt32();
			byte[] responseData = new byte[dataSize];
			_responseBlob.Read(responseData, 0, responseData.Length);
		
			Parameters responseParams = new Parameters();
			responseParams.AddPrimitiveType("data", responseData);
		
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_Unseal, responseParams);
		}
		
		
		public override HMACKeyInfo GetKeyInfo (AuthSessionNum authSessionNum)
		{
			if(authSessionNum == AuthSessionNum.Auth1)
			{
				string keyIdentifier = _params.GetValueOf<string>("key");
				
				Parameters parameters = new Parameters();
				parameters.AddPrimitiveType("identifier", keyIdentifier);
				
				return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyUsageSecret, parameters);
			}
			else if(authSessionNum == AuthSessionNum.Auth2)
			{
				string identifier = _params.GetValueOf<string>("key");
				
				Parameters parameters = new Parameters();
				parameters.AddPrimitiveType("identifier", identifier);
				
				return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.SealAuth, parameters);
			}
			else
				return null;
		}


	
		public override bool SupportsAuthType (AuthHandle.AuthType authType)
		{	
			return authType == AuthHandle.AuthType.OIAP;
		}


		public override string GetHandle (AuthSessionNum authSessionNum)
		{
			return null;
		}

	
	}
}
