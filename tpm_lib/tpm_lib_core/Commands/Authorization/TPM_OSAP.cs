
using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;
using Iaik.Tc.TPM.Library.Common.Handles;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common.KeyData;

namespace Iaik.Tc.TPM.Library.Commands
{

	[TPMCommands(TPMCommandNames.TPM_CMD_OSAP)]
	public class TPM_OSAP : TPMCommand
	{
		

		public override TPMCommandResponse Process ()
		{
			TPMEntityTypeLSB entityLSB = _params.GetValueOf<TPMEntityTypeLSB>("entity_lsb");
			TPMEntityTypeMSB entityMSB = _params.GetValueOf<TPMEntityTypeMSB>("entity_msb");
			string identifier = _params.GetValueOf<string>("entity_value");
			
			
			if( entityLSB != TPMEntityTypeLSB.TPM_ET_KEYHANDLE &&
				entityLSB != TPMEntityTypeLSB.TPM_ET_SRK)
			{
				throw new ArgumentException("TPM_OSAP does currently not support entityType: " + entityLSB.ToString());
			}
			
			if(entityMSB != TPMEntityTypeMSB.TPM_ET_XOR)
			{
				throw new ArgumentException(string.Format("TPM_OSAP does currently not support '{0}' EncAuth encryption", entityMSB));
			}
			
			
			//We now know that the current identifier is a key identifier (maybe srk, but then the value is ignored by TPM_OSAP).
			//So we invoke the key manager to load the key with the specified identifier and establish an OSAP session
			_keyManager.LoadKey(identifier);
			
			//handle is not known yet
			AuthHandle authHandle = new AuthHandle(AuthHandle.AuthType.OSAP, 0);
			authHandle.NewNonceOddOSAP();	
		
			
			using(_keyManager.AcquireLock())
			{
		
				TPMBlob requestBlob = new TPMBlob();
				requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_OSAP);
				requestBlob.WriteUInt16((ushort)(((ushort)entityMSB <<  8) | (ushort)entityLSB));
				
				if(identifier == KeyHandle.KEY_SRK)
					requestBlob.WriteUInt32((uint)TPMKeyHandles.TPM_KH_SRK);
				else
					requestBlob.WriteUInt32(_keyManager.IdentifierToHandle(identifier).Handle);
				
				requestBlob.Write(authHandle.NonceOddOSAP, 0, authHandle.NonceOddOSAP.Length);
				requestBlob.WriteCmdSize();
				
				_responseBlob = _tpmProvider.TransmitAndCheck(requestBlob);
			}
			
			_responseBlob.SkipHeader();
			AuthHandleCore receivedAuthHandle = new AuthHandleCore(AuthHandle.AuthType.OSAP, _responseBlob);			
			authHandle.UpdateFromOtherAuthHandle(receivedAuthHandle);			
			
			
			Parameters parameters = new Parameters();
			parameters.AddValue("auth_handle", authHandle);
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_OIAP, parameters);
		}

		
		public override void Clear ()
		{
		}

	}
}
