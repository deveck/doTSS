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
using System.Runtime.InteropServices;
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;



namespace Iaik.Tc.TPM.Library.Commands
{
	[TPMCommands(TPMCommandNames.TPM_CMD_PCRRead)]
	public sealed class TPM_PCRRead : TPMCommand
	{
		/// <summary>
		/// The PCR register that should be read
		/// </summary>
		private UInt32 _register = 0;
		
		public override void Clear ()
		{
			throw new System.NotImplementedException();
		}
		
		
		
		public TPM_PCRRead() : base(TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_PcrRead)
		{	
		}
	
		public override void Init (Parameters param, TPMProvider provider, TPMWrapper tpmWrapper)
		{
			base.Init(param, provider, tpmWrapper);
			_register = param.GetValueOf<UInt32>("pcrnum");
		}

		public override TPMCommandResponse Process ()
		{
			TPMBlob requestBlob = new TPMBlob ();
			requestBlob.WriteCmdHeader (TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_PcrRead);
			requestBlob.WriteUInt32 ((uint)_register);
			requestBlob.WriteCmdSize ();
			
			TPMBlob responseBlob = TransmitMe (requestBlob);
			Parameters responseParam = new Parameters();
			
			
			byte[] val = responseBlob.ReadBytes(20);
			
			responseParam.AddPrimitiveType("pcrnum", _register);
			responseParam.AddPrimitiveType("value", val);
			
			
			TPMCommandResponse response = new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_PCRRead, responseParam);
			
			return response;
		}
	
		
	}
}
