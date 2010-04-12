
using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Lowlevel;

namespace Iaik.Tc.TPM.Library.Commands.SessionManagement
{


	/// <summary>
	/// Loads the specified context blob and returns the nex handle
	/// </summary>
	[TPMCommands(TPMCommandNames.TPM_CMD_LoadContext)]
	public class TPM_LoadContext : TPMCommand
	{

		public override TPMCommandResponse Process ()
		{
			if(_params.IsDefined<ITPMHandle>("handle") == false ||
			   _params.IsDefined<bool>("keep_handle") == false ||
			   _params.IsDefined<byte[]>("context_blob") == false)
				return new TPMCommandResponse(false, TPMCommandNames.TPM_CMD_LoadContext, new Parameters());
			
			ITPMHandle handle = _params.GetValueOf<ITPMHandle>("handle");
			bool keepHandle = _params.GetValueOf<bool>("keep_handle");
			byte[] contextBlob = _params.GetValueOf<byte[]>("context_blob");
			
			TPMBlob blob = new TPMBlob();
			blob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_LoadContext);
			blob.WriteUInt32(handle.Handle);
			blob.WriteBool(keepHandle);
			blob.WriteUInt32((uint)contextBlob.Length);
			blob.Write(contextBlob, 0, contextBlob.Length);
			
			TPMBlob responseBlob = _tpmProvider.TransmitAndCheck(blob);
			responseBlob.SkipHeader();
			handle.Handle = responseBlob.ReadUInt32();
			
			Parameters responseParameters = new Parameters();
			responseParameters.AddValue("handle", handle);
			
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_LoadContext, responseParameters);
			
		}

	}
}
