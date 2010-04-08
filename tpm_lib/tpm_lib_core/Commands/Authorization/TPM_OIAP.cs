
using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;

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
			
			TPMBlob responseBlob = _tpmProvider.TransmitAndCheck(requestBlob);
			responseBlob.SkipHeader();
			AuthHandleCore authHandle = new AuthHandleCore(responseBlob);
			
			Parameters parameters = new Parameters();
			parameters.AddValue("auth_handle", authHandle);
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_OIAP, parameters);
		}

		
		public override void Clear ()
		{
		}

	}
}
