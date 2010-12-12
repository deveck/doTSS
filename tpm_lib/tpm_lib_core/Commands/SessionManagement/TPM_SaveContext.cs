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
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Library.Common.Handles;
using System.Text;

namespace Iaik.Tc.TPM.Library.Commands.SessionManagement
{

	/// <summary>
	/// Requests the TPM to export a specified resource from the TPM
	/// </summary>
	[TPMCommands(TPMCommandNames.TPM_CMD_SaveContext)]
	public class TPM_SaveContext : TPMCommand
	{

		public override TPMCommandResponse Process ()
		{
			//We don't have any meaningful labeldata we could include,
			//so generate some random
			byte[] labelData = new byte[16];
			Random r = new Random();
			r.NextBytes(labelData);
			
			
			if(_params.IsDefined<ITPMHandle>("handle") == false)
				return new TPMCommandResponse(false, TPMCommandNames.TPM_CMD_SaveContext, new Parameters());
			
			
			ITPMHandle handle = _params.GetValueOf<ITPMHandle>("handle");
			
			TPMBlob requestBlob = new TPMBlob();
			requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_SaveContext);
			requestBlob.WriteUInt32(handle.Handle);
			requestBlob.WriteUInt32((uint)handle.ResourceType);
			requestBlob.Write(labelData, 0, labelData.Length);
			
			TPMBlob responseBlob = TransmitMe(requestBlob);
			responseBlob.SkipHeader();
			
			uint blobSize = responseBlob.ReadUInt32();
			byte[] contextBlob = responseBlob.ReadBytes((int)blobSize);
			
			Parameters responseParams = new Parameters();
			responseParams.AddPrimitiveType("context_blob", contextBlob);
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_SaveContext, responseParams);
			
		}

		public override string GetCommandInternalsBeforeExecute ()
		{
			StringBuilder internals = new StringBuilder();
			internals.AppendLine(_params.GetValueOf<ITPMHandle>("handle").ToString());
			return internals.ToString();
		}

	
	}
}
