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
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.library.exceptions;
using Iaik.Tc.TPM.Library.HandlesCore;

namespace Iaik.Tc.TPM.Library.Commands
{
	[TPMCommands(TPMCommandNames.TPM_CMD_GetCapability)]
	public sealed class TPM_GetCapability : TPMCommand
	{
		/// <summary>
		/// Partition of capabilities to be interrogated
		/// </summary>
		private CapabilityData.TPMCapabilityArea _capArea;
		
		/// <summary>
		/// Parameters need to be cached 
		/// </summary>
		private Parameters _param;
		
		public override void Init (Parameters param, TPMProvider tpmProvider, TPMWrapper tpmWrapper)
		{
			base.Init (param, tpmProvider, tpmWrapper);

			_param = param;
			_capArea = param.GetValueOf<CapabilityData.TPMCapabilityArea> ("capArea");
		}
		
		public override TPMCommandResponse Process ()
		{
			TPMBlob requestBlob = new TPMBlob ();
			requestBlob.WriteCmdHeader (TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_GetCapability);
			requestBlob.WriteUInt32 ((uint)_capArea);
			
			switch(_capArea)
			{
			case CapabilityData.TPMCapabilityArea.TPM_CAP_VERSION_VAL:
				//Subcaps are ignored by TPM_CAP_VERSION_VAL
				requestBlob.WriteUInt32 (0);
				break;
			case CapabilityData.TPMCapabilityArea.TPM_CAP_HANDLE:
				requestBlob.WriteUInt32 (4);
				requestBlob.WriteUInt32 ((uint)_param.GetValueOf<TPMResourceType> ("handle_type"));
				break;
			case CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY:
				CapabilityData.TPMSubCapProperty subCap = _param.GetValueOf<CapabilityData.TPMSubCapProperty> ("subCap");
				
				//Size of subcap
				requestBlob.WriteUInt32 (4);
				requestBlob.WriteUInt32 ((uint)subCap);
				break;
			case CapabilityData.TPMCapabilityArea.TPM_CAP_SELECT_SIZE:
				
				CapabilityDataCore.TPMSelectSizeCore tpmSelectSize = 
					CapabilityDataCore.TPMSelectSizeCore.CreateVersion12(_param.GetValueOf<ushort>(CapabilityData.PARAM_PROP_SELECT_SIZE));
				
				TPMBlobWriteableHelper.WriteITPMBlobWritableWithUIntSize(requestBlob, tpmSelectSize);
				break;
				
			default:
				throw new NotSupportedException ("Defined cap or subcap are not supported");
			}
				
			
			
//			if (_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_VERSION_VAL)
//			{
//				//Subcaps are ignored by TPM_CAP_VERSION_VAL
//				requestBlob.WriteUInt32 (0);
//			}
//			else if (_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_HANDLE)
//			{
//				requestBlob.WriteUInt32 (4);
//				requestBlob.WriteUInt32 ((uint)_param.GetValueOf<TPMResourceType> ("handle_type"));
//			}
//			else if (_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY)
//			{
//				CapabilityData.TPMSubCapProperty subCap = _param.GetValueOf<CapabilityData.TPMSubCapProperty> ("subCap");
//				
//				//Size of subcap
//				requestBlob.WriteUInt32 (4);
//				requestBlob.WriteUInt32 ((uint)subCap);
//			}
//			else
//				throw new NotSupportedException ("Defined cap or subcap are not supported");
			
			requestBlob.WriteCmdSize ();
			
			TPMBlob responseBlob = TransmitMe(requestBlob);
			responseBlob.SkipHeader ();
			
			Parameters parameters = new Parameters ();
			
			
			switch(_capArea)
			{
			case CapabilityData.TPMCapabilityArea.TPM_CAP_VERSION_VAL:
				CapabilityDataCore.TPMCapVersionInfoCore tpmVersionInfo = new CapabilityDataCore.TPMCapVersionInfoCore (responseBlob);
				parameters.AddValue (CapabilityData.PARAM_TPM_VERSION_INFO, tpmVersionInfo);
				break;
			case CapabilityData.TPMCapabilityArea.TPM_CAP_HANDLE:
				responseBlob.SkipHeader ();
				
				//Reads the response size, which is ignored
				responseBlob.ReadUInt32 ();
				
				HandleListCore handleList = new HandleListCore (responseBlob, _param.GetValueOf<TPMResourceType> ("handle_type"));
				parameters.AddValue ("handles", handleList);
				break;
			case CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY:
				responseBlob.SkipHeader ();
				switch(_param.GetValueOf<CapabilityData.TPMSubCapProperty> ("subCap"))
				{
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_PCR:
					parameters.AddPrimitiveType (CapabilityData.PARAM_PROP_PCR, ReadUInt32Response (responseBlob));
					break;
                //case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_DIR:
                //    parameters.AddPrimitiveType (CapabilityData.PARAM_PROP_DIR, ReadUInt32Response (responseBlob));
                //    break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MANUFACTURER:
					parameters.AddPrimitiveType (CapabilityData.PARAM_PROP_MANUFACTURER, ReadUInt32Response (responseBlob));
					break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_KEYS:
					parameters.AddPrimitiveType (CapabilityData.PARAM_PROP_KEYS, ReadUInt32Response (responseBlob));
					break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_AUTHSESS:
					parameters.AddPrimitiveType (CapabilityData.PARAM_PROP_MAX_AUTHSESS, ReadUInt32Response (responseBlob));
					break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_TRANSESS:
					parameters.AddPrimitiveType (CapabilityData.PARAM_PROP_MAX_TRANSESS, ReadUInt32Response (responseBlob));
					break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_KEYS:
					parameters.AddPrimitiveType (CapabilityData.PARAM_PROP_MAX_KEYS, ReadUInt32Response (responseBlob));
					break;
				case CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_SESSIONS:
					parameters.AddPrimitiveType (CapabilityData.PARAM_PROP_MAX_SESSIONS, ReadUInt32Response (responseBlob));
					break;
				default:
					throw new NotSupportedException("Defined cap or subcap are not supported");
				}
				break;
				
			case CapabilityData.TPMCapabilityArea.TPM_CAP_SELECT_SIZE:
				parameters.AddPrimitiveType(CapabilityData.PARAM_PROP_SELECT_SIZE, ReadBoolResponse(responseBlob));
				break;
			}
			
			
			
//			if (_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_VERSION_VAL)
//			{
//				CapabilityDataCore.TPMCapVersionInfoCore tpmVersionInfo = new CapabilityDataCore.TPMCapVersionInfoCore (responseBlob);
//				parameters.AddValue (CapabilityData.PARAM_TPM_VERSION_INFO, tpmVersionInfo);
//			}
//			else if (_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_HANDLE)
//			{
//				responseBlob.SkipHeader ();
//				
//				//Reads the response size, which is ignored
//				responseBlob.ReadUInt32 ();
//				
//				HandleListCore handleList = new HandleListCore (responseBlob, _param.GetValueOf<TPMResourceType> ("handle_type"));
//				parameters.AddValue ("handles", handleList);
//			}
//			
//			
//			else if (_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY && 
//				    _param.GetValueOf<CapabilityData.TPMSubCapProperty> ("subCap") == CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_PCR) 
//			{
//				responseBlob.SkipHeader ();
//				parameters.AddPrimitiveType (CapabilityData.PARAM_PROP_PCR, ReadUInt32Response (responseBlob));
//			}
//			
//			else if (_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY && 
//				_param.GetValueOf<CapabilityData.TPMSubCapProperty> ("subCap") == CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_AUTHSESS) 
//			{
//				responseBlob.SkipHeader ();
//				parameters.AddPrimitiveType (CapabilityData.PARAM_PROP_MAX_AUTHSESS, ReadUInt32Response (responseBlob));
//			}
//			
//			else if (_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY && 
//				_param.GetValueOf<CapabilityData.TPMSubCapProperty> ("subCap") == CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_TRANSESS) 
//			{
//				responseBlob.SkipHeader ();
//				parameters.AddPrimitiveType (CapabilityData.PARAM_PROP_MAX_TRANSESS, ReadUInt32Response (responseBlob));
//			}
//			
//			else if (_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY && 
//				_param.GetValueOf<CapabilityData.TPMSubCapProperty> ("subCap") == CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_SESSIONS) 
//			{
//				responseBlob.SkipHeader ();
//				parameters.AddPrimitiveType (CapabilityData.PARAM_PROP_MAX_SESSIONS, ReadUInt32Response (responseBlob));
//			}
//			else
//				throw new NotSupportedException("Defined cap or subcap are not supported");
			
			return new TPMCommandResponse (true, TPMCommandNames.TPM_CMD_GetCapability, parameters);
		}
		
		private uint ReadUInt32Response (TPMBlob response)
		{
			uint responseSize = response.ReadUInt32 ();
			if (responseSize != 4)
				throw new TPMResponseException (string.Format ("Capability response size mismatch (should be 4, and is {0})", responseSize));
			
			return response.ReadUInt32 ();
		}
		
		private bool ReadBoolResponse(TPMBlob response)
		{
			uint responseSize = response.ReadUInt32 ();
			if (responseSize != 1)
				throw new TPMResponseException (string.Format ("Capability response size mismatch (should be 1, and is {0})", responseSize));
			
			return response.ReadBool ();
		}
				
		public override void Clear ()
		{
		}
	}
}
