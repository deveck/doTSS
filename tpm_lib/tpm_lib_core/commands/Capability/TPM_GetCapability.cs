// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;

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
		/// Further definition of information
		/// </summary>
		private byte[] _subCap;
		
		/// <summary>
		/// Parameters need to be cached 
		/// </summary>
		private Parameters _param;
		
		public override void Init (Parameters param, TPMProvider tpmProvider)
		{
			base.Init (param, tpmProvider);

			_param = param;
			_capArea = param.GetValueOf<CapabilityData.TPMCapabilityArea> ("capArea");
		}
		
		public override TPMCommandResponse Process ()
		{
			TPMBlob requestBlob = new TPMBlob ();
			requestBlob.WriteCmdHeader (TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_GetCapability);
			requestBlob.WriteUInt32 ((uint)_capArea);
			
			if(_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_VERSION_VAL)
			{
				//Subcaps are ignored by TPM_CAP_VERSION_VAL
				requestBlob.WriteUInt32(0);
			}
			else if(_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY)
			{	
				CapabilityData.TPMSubCapProperty subCap = _param.GetValueOf<CapabilityData.TPMSubCapProperty>("subCap");
				
				//Size of subcap
				requestBlob.WriteUInt32(4);
				requestBlob.WriteUInt32((uint)subCap);
			}
			else
				throw new NotSupportedException("Defined cap or subcap are not supported");
			
			requestBlob.WriteCmdSize ();
			
			TPMBlob responseBlob = _tpmProvider.TransmitAndCheck (requestBlob);
			responseBlob.SkipHeader ();
			
			Parameters parameters = new Parameters ();
			
			
			if(_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_VERSION_VAL)
			{
				CapabilityDataCore.TPMCapVersionInfoCore tpmVersionInfo = new CapabilityDataCore.TPMCapVersionInfoCore (responseBlob);
				parameters.AddValue (CapabilityData.PARAM_TPM_VERSION_INFO, tpmVersionInfo);
			}
			else if(_capArea == CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY && 
				    _param.GetValueOf<CapabilityData.TPMSubCapProperty>("subCap") == CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_PCR)					
			{
				parameters.AddPrimitiveType(CapabilityData.PARAM_PROP_PCR, 0);
			}
			else
				throw new NotSupportedException("Defined cap or subcap are not supported");
			
			return new TPMCommandResponse (true, TPMCommandNames.TPM_CMD_GetCapability, parameters);
		}
		
		public override void Clear ()
		{
		}
	}
}
