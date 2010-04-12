
using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Library.Common.Handles;

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
			
			TPMBlob responseBlob =_tpmProvider.TransmitAndCheck(requestBlob);
			responseBlob.SkipHeader();
			
			uint blobSize = responseBlob.ReadUInt32();
			byte[] contextBlob = responseBlob.ReadBytes((int)blobSize);
			
			Parameters responseParams = new Parameters();
			responseParams.AddPrimitiveType("context_blob", contextBlob);
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_SaveContext, responseParams);
			
		}

	
	}
}
