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
using System.IO;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Hash;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Library.Commands.StorageFunctions
{

	[TPMCommands(TPMCommandNames.TPM_CMD_LoadKey2)]
	public class TPM_LoadKey2 : TPMCommandAuthorizable
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
		/// Key specification of the key being loaded
		/// </summary>
		private TPMKeyCore _tpmKey = null;
	
		public override byte[] Digest 
		{
			get 
			{
				if(_digest == null)
				{
					_digest = new HashProvider().Hash(
						new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_LoadKey2),
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
					
					_responseDigest = hasher.Hash(
					      //1S
					      new HashStreamDataProvider(_responseBlob, offset, 4, false),
					      //2S
					      new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_LoadKey2));
				}
				
				return _responseDigest;
			}
		}


		public override void Init (Parameters param, TPMProvider tpmProvider, TPMWrapper tpmWrapper)
		{
			base.Init (param, tpmProvider, tpmWrapper);
			
			_digest = null;			
			_tpmKey =  TPMKeyCore.CreateFromBytes(param.GetValueOf<byte[]>("key_blob"));
		}


		protected override TPMCommandResponse InternalProcess ()
		{
			TPMBlob requestBlob = new TPMBlob ();
			requestBlob.WriteCmdHeader (TPMCmdTags.TPM_TAG_RQU_AUTH1_COMMAND, TPMOrdinals.TPM_ORD_LoadKey2);			
			
			//If not loaded load now
			if(_params.GetValueOf<bool>("parent_key_srk") == false)
				_keyManager.LoadKey(_params.GetValueOf<string>("parent_identifier"));
						
			//To be inserted later
			requestBlob.WriteUInt32 (0);
			
			_tpmKey.WriteToTpmBlob(requestBlob);
			
			//Blocking authorize, blocks till the user has entered the authorization data
			AuthorizeMe(requestBlob);

			using(_keyManager.AcquireLock())
			{
				_keyManager.EnsureFreeSlot();
				uint tpmKeyHandle;
				
				if(_params.GetValueOf<bool>("parent_key_srk"))
					tpmKeyHandle = (uint)TPMKeyHandles.TPM_KH_SRK;
				else
					tpmKeyHandle = _keyManager.IdentifierToHandle(_params.GetValueOf<string>("parent_identifier")).Handle;
				
				//Write key handle to the first position after the header
				requestBlob.SkipHeader();
				requestBlob.WriteUInt32(tpmKeyHandle);

				_responseBlob = TransmitMe(requestBlob);
			}

			CheckResponseAuthInfo();			
			
			_responseBlob.SkipHeader();
			uint loadedTpmHandle = _responseBlob.ReadUInt32();
			KeyHandle loadedHandle = new KeyHandle(_params.GetValueOf<string>("key_identifier"), loadedTpmHandle);
			
			_responseParameters = new Parameters();
			_responseParameters.AddPrimitiveType("handle", loadedHandle);
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_LoadKey2, _responseParameters);
		}
		
		
		public override HMACKeyInfo GetKeyInfo (AuthSessionNum authSessionNum)
		{
			if(authSessionNum != AuthSessionNum.Auth1)
				return null;
		
		
			if(_params.GetValueOf<bool>("parent_key_srk"))
				return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.SrkSecret, new Parameters());
			else
			{
				Parameters hmacKeyInfoParams = new Parameters();
				hmacKeyInfoParams.AddPrimitiveType("identifier", _params.GetValueOf<string>("parent_identifier"));
				return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyUsageSecret, hmacKeyInfoParams);
			}
		}


	
		public override bool SupportsAuthType (AuthHandle.AuthType authType)
		{
			return authType == AuthHandle.AuthType.OIAP;
		}
		
		public override string GetCommandInternalsBeforeExecute ()
		{
			uint tpmKeyHandle;
			if(_params.GetValueOf<bool>("parent_key_srk"))
				tpmKeyHandle = (uint)TPMKeyHandles.TPM_KH_SRK;
			else
				tpmKeyHandle = _keyManager.IdentifierToHandle(_params.GetValueOf<string>("parent_identifier")).Handle;

			
			return string.Format("{0}\tparent_handle=0x{1:X}",base.GetCommandInternalsBeforeExecute (),tpmKeyHandle);
				
		}

	}
}
