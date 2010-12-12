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


	[TPMCommands(TPMCommandNames.TPM_CMD_ReadCounter)]
	public class TPM_ReadCounter : TPMCommand
	{		
		public override TPMCommandResponse Process ()
		{
			using(TPMBlob requestBlob = new TPMBlob())
			{
				requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_ReadCounter);
				requestBlob.WriteUInt32(_params.GetValueOf<uint>("counter_id"));
				
				_responseBlob = TransmitMe(requestBlob);				
			}
			
			_responseBlob.SkipHeader();
			_responseParameters = new Parameters();
			_responseParameters.AddPrimitiveType("counter_id", _params.GetValueOf<uint>("counter_id"));
			_responseParameters.AddValue("counter_value", TPMCounterValueCore.CreateFromTPMBlob(_responseBlob));
			
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_ReadCounter, _responseParameters);
		}
	}
}
