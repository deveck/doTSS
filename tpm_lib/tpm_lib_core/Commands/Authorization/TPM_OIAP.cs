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
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;

namespace Iaik.Tc.TPM.Library.Commands
{

	[TPMCommands(TPMCommandNames.TPM_CMD_OIAP)]
	public class TPM_OIAP : TPMCommand
	{
		

		public override TPMCommandResponse Process ()
		{
			TPMBlob requestBlob = new TPMBlob();
			requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_OIAP);
			requestBlob.WriteCmdSize();

            _commandAuthHelper.EnsureFreeSlot();
			TPMBlob responseBlob = TransmitMe(requestBlob);
			responseBlob.SkipHeader();
			AuthHandleCore authHandle = new AuthHandleCore(AuthHandle.AuthType.OIAP, responseBlob);
			
			_responseParameters = new Parameters();
			_responseParameters.AddValue("auth_handle", authHandle);
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_OIAP, _responseParameters);
		}
		
		public override void Clear ()
		{
		}

	}
}
