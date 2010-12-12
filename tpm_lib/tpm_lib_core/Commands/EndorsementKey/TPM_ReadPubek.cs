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



using System;
using Iaik.Tc.TPM.Library.Commands;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Utils.Nonce;
using Iaik.Tc.TPM.Library.KeyDataCore;
using Iaik.Tc.TPM.Library.CommonTPMDataTypes;
using Iaik.Utils.Hash;

namespace Iaik.Tc.TPM.Library.Commands
{


	/// <summary>
	/// Returns the public portion of the endorsement key
	/// </summary>
	[TPMCommands(TPMCommandNames.TPM_CMD_ReadPubek)]
	public class TPM_ReadPubek : TPMCommand
	{

		public override void Clear ()
		{
		}

		
		public override TPMCommandResponse Process ()
		{
			byte[] nonce = NonceGenerator.GenerateByteNonce (20);
			
			TPMBlob requestBlob = new TPMBlob ();
			requestBlob.WriteCmdHeader (TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_ReadPubek);
			requestBlob.Write (nonce, 0, nonce.Length);
			requestBlob.WriteCmdSize ();
			
			TPMBlob responseBlob = TransmitMe(requestBlob);
			responseBlob.SkipHeader ();
			
			long posStart = responseBlob.Position;
			TPMPubkeyCore pubkey = TPMPubkeyCore.CreateFromTPMBlob(responseBlob);
			long posEnd = responseBlob.Position;
			
			Digest digest = new Digest (responseBlob, 20);
			if (digest.CompareTo (
				new HashStreamDataProvider (responseBlob, posStart, posEnd - posStart, false),
				new HashByteDataProvider (nonce)) == false)
			{
				throw new TPMResponseException ("Local digest does not match remote digest");
			}			
			
			Parameters responseParams = new Parameters ();
			responseParams.AddValue (TPMPubkey.PARAM_TPM_PUBKEY, pubkey);
			
			return new TPMCommandResponse (true, TPMCommandNames.TPM_CMD_ReadPubek, responseParams);
			
		}

		
	}
}
