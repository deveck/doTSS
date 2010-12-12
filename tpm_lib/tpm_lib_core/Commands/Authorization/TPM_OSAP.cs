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
				entityLSB != TPMEntityTypeLSB.TPM_ET_SRK &&
				entityLSB != TPMEntityTypeLSB.TPM_ET_OWNER)
			{
				throw new ArgumentException("TPM_OSAP does currently not support entityType: " + entityLSB.ToString());
			}
			
			if(entityMSB != TPMEntityTypeMSB.TPM_ET_XOR)
			{
				throw new ArgumentException(string.Format("TPM_OSAP does currently not support '{0}' EncAuth encryption", entityMSB));
			}
			
			
			if(entityLSB == TPMEntityTypeLSB.TPM_ET_KEYHANDLE ||
			   entityLSB == TPMEntityTypeLSB.TPM_ET_SRK)
			{
				//We now know that the current identifier is a key identifier (maybe srk, but then the value is ignored by TPM_OSAP).
				//So we invoke the key manager to load the key with the specified identifier and establish an OSAP session
				_keyManager.LoadKey(identifier);
			}
			
			//handle is not known yet
			AuthHandle authHandle = new AuthHandle(AuthHandle.AuthType.OSAP, 0);
            authHandle.EntityType = entityLSB;
			authHandle.NewNonceOddOSAP();	
		
			
			using(_keyManager.AcquireLock())
			{
		
				TPMBlob requestBlob = new TPMBlob();
				requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_OSAP);
				requestBlob.WriteUInt16((ushort)(((ushort)entityMSB <<  8) | (ushort)entityLSB));
				
				if(entityLSB == TPMEntityTypeLSB.TPM_ET_KEYHANDLE ||
				   entityLSB == TPMEntityTypeLSB.TPM_ET_SRK)
				{
                    if (identifier == KeyHandle.KEY_SRK)
                    {
                        requestBlob.WriteUInt32((uint)TPMKeyHandles.TPM_KH_SRK);
                        authHandle.EntityValue = (uint)TPMKeyHandles.TPM_KH_SRK;
                    }
                    else
                    {
                        KeyHandle keyHandle = _keyManager.IdentifierToHandle(identifier);
                        requestBlob.WriteUInt32(keyHandle.Handle);
                        authHandle.EntityValue = keyHandle.Handle;
                    }
				}
				else if(entityLSB == TPMEntityTypeLSB.TPM_ET_OWNER)
				{
					requestBlob.WriteUInt32((uint)TPMKeyHandles.TPM_KH_OWNER);
                    authHandle.EntityValue = (uint)TPMKeyHandles.TPM_KH_OWNER;
				}
				
				requestBlob.Write(authHandle.NonceOddOSAP, 0, authHandle.NonceOddOSAP.Length);
				requestBlob.WriteCmdSize();

                _commandAuthHelper.EnsureFreeSlot();
				_responseBlob = TransmitMe(requestBlob);
			}
			
			_responseBlob.SkipHeader();
			AuthHandleCore receivedAuthHandle = new AuthHandleCore(AuthHandle.AuthType.OSAP, _responseBlob);			
			authHandle.UpdateFromOtherAuthHandle(receivedAuthHandle);			
			
			
			_responseParameters = new Parameters();
			_responseParameters.AddValue("auth_handle", authHandle);
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_OSAP, _responseParameters);
		}

		
		public override void Clear ()
		{
		}
		
//		public override string GetCommandInternalsBeforeExecute ()
//		{			
//			return string.Format("entity_lsb={0}, entity_msb={1}, KeyIdentifier={2}", 			                     
//			 _params.GetValueOf<TPMEntityTypeLSB>("entity_lsb"),
//			 _params.GetValueOf<TPMEntityTypeMSB>("entity_msb"),
//			 _params.GetValueOf<string>("entity_value"));
//		}
//		
//		public override string GetCommandInternalsAfterExecute ()
//		{
//			return _responseParameters.ToString();
//		}




	}
}
