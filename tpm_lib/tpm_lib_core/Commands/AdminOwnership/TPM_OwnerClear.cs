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
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;

namespace Iaik.Tc.TPM.Library.Commands.AdminOwnership
{

	

	/// <summary>
	/// The TPM_OwnerClear command performs the clear operation under Owner authentication.
	/// This command is available until the Owner executes the TPM_DisableOwnerClear, at which
	/// time any further invocation of this command returns TPM_CLEAR_DISABLED.
	/// </summary>
	[TPMCommands(TPMCommandNames.TPM_CMD_OwnerClear)]
	public class TPM_OwnerClear : TPMCommandAuthorizable
	{
		/// <summary>
		/// Digest of the in parameters
		/// </summary>
		private byte[] _digest = null;
		
		/// <summary>
		/// Digest of the out parameters
		/// </summary>
		private byte[] _responseDigest = null;
		
		public override byte[] Digest 
		{
			get 
			{ 
				if(_digest == null)
				{
					HashProvider hashProvider = new HashProvider();
					_digest = hashProvider.Hash(new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_OwnerClear));
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
					      new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_OwnerClear));

				}
				
				return _responseDigest;
				
			}
		}
		

		
		protected override TPMCommandResponse InternalProcess ()
		{
			TPMBlob ownerClearRequest = new TPMBlob();
			ownerClearRequest.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_AUTH1_COMMAND, TPMOrdinals.TPM_ORD_OwnerClear);
			
			_responseBlob = AuthorizeMeAndTransmit(ownerClearRequest);	
		
			CheckResponseAuthInfo();			
			
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_OwnerClear, new Parameters());

		}

		
		public override HMACKeyInfo GetKeyInfo (AuthSessionNum authSessionNum)
		{
			if(authSessionNum == AuthSessionNum.Auth1)
				return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.OwnerSecret, new Parameters());
			
			return null;
		}
		
		public override bool SupportsAuthType (AuthHandle.AuthType authType)
		{
			return authType == AuthHandle.AuthType.OIAP;
		}
		


	}
}
