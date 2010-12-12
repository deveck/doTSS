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
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Utils;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common.Basic;
using Iaik.Tc.TPM.Library.Basic;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;

namespace Iaik.Tc.TPM.Library.Commands.MonotonicCounter
{


	[TPMCommands(TPMCommandNames.TPM_CMD_CreateCounter)]
	public class TPM_CreateCounter : TPMCommandAuthorizable
	{
		/// <summary>
		/// Contains the Secret (encauth) for the counter
		/// </summary>
		private byte[] _secret;

		/// <summary>
		/// Label of the counter, what is this used for?
		/// </summary>
		private byte[] _label;


		/// <summary>
		/// The outgoing digest for this command
		/// </summary>
		private byte[] _digest = null;
	
	
		/// <summary>
		/// Digest of the tpm response
		/// </summary>
		private byte[] _responseDigest = null;
	
	
		public override byte[] Digest 
		{
			get 
			{
				if(_digest == null)
				{
					_digest = new HashProvider().Hash(
						new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_CreateCounter),
						new HashByteDataProvider(_secret),
						new HashByteDataProvider(_label));
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
					      new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_CreateCounter),
					      //3S
					      new HashStreamDataProvider(_responseBlob, offset + 4, _responseBlob.Length - offset - 4 - authHandleSize, false));
				}
				
				return _responseDigest;
			}
		}

	

		public override void Init (Parameters param, TPMProvider tpmProvider, TPMWrapper tpmWrapper)
		{
			base.Init (param, tpmProvider, tpmWrapper);
			
			_secret = _params.GetValueOf<byte[]>("secret");
			_label = _params.GetValueOf<byte[]>("label");
		}

		
		protected override TPMCommandResponse InternalProcess ()
		{
			AuthHandle auth1OSAP =_commandAuthHelper.AssureOSAPSharedSecret(this, AuthSessionNum.Auth1);			
			
			byte[] xorKey = new HashProvider().Hash(
					new HashByteDataProvider(auth1OSAP.SharedSecret),
					new HashByteDataProvider(auth1OSAP.NonceEven));
					
			ByteHelper.XORBytes(_secret, xorKey);
			
			
			if(_secret.Length != 20)
				throw new ArgumentException("secret needs to be 20 bytes long (SHA1 hash)");
			
			if(_label.Length != 4)
				throw new ArgumentException("Label needs to be 4 bytes long");
			
			using(TPMBlob requestBlob = new TPMBlob())
			{
				requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_AUTH1_COMMAND, TPMOrdinals.TPM_ORD_CreateCounter);
				requestBlob.Write(_secret, 0, 20);
				requestBlob.Write(_label, 0, 4);
				
				_responseBlob = AuthorizeMeAndTransmit(requestBlob);				
			}
			
			_responseBlob.SkipHeader();
			_responseParameters = new Parameters();
			_responseParameters.AddPrimitiveType("counter_id", _responseBlob.ReadUInt32());
			_responseParameters.AddValue("counter_value", TPMCounterValueCore.CreateFromTPMBlob(_responseBlob));
			
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_CreateCounter, _responseParameters);
		}
		
		public override HMACKeyInfo GetKeyInfo (AuthSessionNum authSessionNum)
		{
			if(authSessionNum == AuthSessionNum.Auth1)
				return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.OwnerSecret, new Parameters());
				
			return null;
		}
		
		public override bool SupportsAuthType (AuthHandle.AuthType authType)
		{
			return authType == AuthHandle.AuthType.OSAP;
		}

		public override string GetHandle (AuthSessionNum authSessionNum)
		{
			if(authSessionNum == AuthSessionNum.Auth1)
				return TPMKeyHandles.TPM_KH_OWNER.ToString();
			
			return null;
		}

		public override TPMEntityTypeLSB GetEntityType (AuthSessionNum austhSessionNum)
		{
			return TPMEntityTypeLSB.TPM_ET_OWNER;
		}


	}
}
