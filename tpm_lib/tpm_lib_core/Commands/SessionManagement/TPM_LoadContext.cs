
using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Lowlevel;
using System.Text;

namespace Iaik.Tc.TPM.Library.Commands.SessionManagement
{


	/// <summary>
	/// Loads the specified context blob and returns the new handle
	/// </summary>
    /// <remarks>
    /// There is nothing special to do for OIAP sessions and Keys,
    /// but for OSAP (and DSAP) we need to make sure that the associated handle (e.g. key handle) is available and swapped in.
    /// If the OSAP session is associated with a key handle, and the key manager can not restore the same key handle, we need to build 
    /// a new OSAP session. This is done in the auth handle manager
    /// </remarks>
	[TPMCommands(TPMCommandNames.TPM_CMD_LoadContext)]
	public class TPM_LoadContext : TPMCommand
	{

		public override TPMCommandResponse Process ()
		{
			if(_params.IsDefined<ITPMHandle>("handle") == false ||
			   _params.IsDefined<byte[]>("context_blob") == false)
				return new TPMCommandResponse(false, TPMCommandNames.TPM_CMD_LoadContext, new Parameters());
			
			ITPMHandle handle = _params.GetValueOf<ITPMHandle>("handle");
			
			
			TPMBlob blob = new TPMBlob();
			blob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_LoadContext);
			blob.WriteUInt32(handle.Handle);
			blob.WriteBool(handle.ForceHandle);
			blob.WriteUInt32((uint)handle.ContextBlob.Length);
			blob.Write(handle.ContextBlob, 0, handle.ContextBlob.Length);
			
			TPMBlob responseBlob = TransmitMe(blob);
			responseBlob.SkipHeader();
			handle.Handle = responseBlob.ReadUInt32();
			
			Parameters responseParameters = new Parameters();
			responseParameters.AddValue("handle", handle);
			
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_LoadContext, responseParameters);			
		}
		
		public override string GetCommandInternalsBeforeExecute ()
		{
			StringBuilder internals = new StringBuilder();
			internals.AppendLine(_params.GetValueOf<ITPMHandle>("handle").ToString());
			return internals.ToString();
		}


	}
}
