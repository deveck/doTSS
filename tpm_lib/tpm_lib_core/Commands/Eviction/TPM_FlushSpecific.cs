
using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Lowlevel;

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
			
			TPMBlob responseBlob = _tpmProvider.TransmitAndCheck (requestBlob);
			
			return new TPMCommandResponse (true, TPMCommandNames.TPM_CMD_FlushSpecific, new Parameters ());
		}

		
		
		public override void Clear ()
		{
		}

	}
}
