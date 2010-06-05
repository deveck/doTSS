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
