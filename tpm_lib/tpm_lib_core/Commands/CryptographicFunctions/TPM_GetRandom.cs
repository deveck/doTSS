// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Lowlevel;

namespace Iaik.Tc.TPM.Library.Commands.CryptographicFunctions
{

	[TPMCommands(TPMCommandNames.TPM_CMD_GetRandom)]
	public class TPM_GetRandom : TPMCommand
	{

		public override TPMCommandResponse Process ()
		{
			using(TPMBlob requestBlob = new TPMBlob())
			{
				requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_GetRandom);
				requestBlob.WriteUInt32(_params.GetValueOf<uint>("bytes_requested"));
				_responseBlob = TransmitMe(requestBlob);
			}
			
			_responseBlob.SkipHeader();
			uint responseByteSize = _responseBlob.ReadUInt32();
			_responseParameters = new Parameters();
			_responseParameters.AddPrimitiveType("data", _responseBlob.ReadBytes((int)responseByteSize));
			
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_GetRandom, _responseParameters);
		}

	}
}
