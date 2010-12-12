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
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Lowlevel;
using System.Text;

namespace Iaik.Tc.TPM.Library.Commands.Eviction
{

	/// <summary>
	/// TPM_FlushSpecific. Flushes the specified handle from the TPM
	/// </summary>
	[TPMCommands(TPMCommandNames.TPM_CMD_FlushSpecific)]
	public class TPM_FlushSpecific : TPMCommand
	{

		
		public override TPMCommandResponse Process ()
		{
			ITPMHandle handle = _params.GetValueOf<ITPMHandle> ("handle");
			
			TPMBlob requestBlob = new TPMBlob ();
			requestBlob.WriteCmdHeader (TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_FlushSpecific);
			requestBlob.WriteUInt32 (handle.Handle);
			requestBlob.WriteUInt32 ((uint)handle.ResourceType);
			
			requestBlob.WriteCmdSize ();
			
			try
			{
				TransmitMe(requestBlob);
			}
			catch(Exception)
			{
				if(!_params.GetValueOf<bool>("ignore_tpm_error", false))
					throw;
			}
			
			return new TPMCommandResponse (true, TPMCommandNames.TPM_CMD_FlushSpecific, new Parameters ());
		}

		
		
		public override void Clear ()
		{
		}

		
		public override string GetCommandInternalsAfterExecute ()
		{
			StringBuilder internals = new StringBuilder();
			internals.AppendLine(_params.GetValueOf<ITPMHandle>("handle").ToString());
			return internals.ToString();
		}

	}
}
