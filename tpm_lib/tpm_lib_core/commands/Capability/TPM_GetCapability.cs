// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.library;
using Iaik.Tc.Tpm.library.common;
using Iaik.Tc.Tpm.lowlevel;
using Iaik.Tc.Tpm.lowlevel.data;

namespace Iaik.Tc.Tpm.library.commands
{
	[TpmCommands(TPMCommandNames.TPM_CMD_GetCapability)]
	public sealed class TPM_GetCapability : TpmCommand
	{
		/// <summary>
		/// Partition of capabilities to be interrogated
		/// </summary>
		private CapabilityData.TPMCapabilityArea _capArea;
		
		/// <summary>
		/// Further definition of information
		/// </summary>
		private byte[] _subCap;
		
		public override void Init (Parameters param, TPMProvider tpmProvider)
		{
			base.Init (param, tpmProvider);
			
			_capArea = param.GetValueOf<CapabilityData.TPMCapabilityArea> ("capArea");
			_subCap = param.GetValueOf<byte[]> ("subCap");
		}
		
		public override void Process ()
		{
			TpmBlob requestBlob = new TpmBlob ();
			requestBlob.WriteCmdHeader (TpmCmdTags.TPM_TAG_RQU_COMMAND, TpmOrdinals.TPM_ORD_GetCapability);
			requestBlob.WriteUInt32 ((uint)_capArea);
			requestBlob.WriteUInt32 ((uint)_subCap.Length);
			requestBlob.Write (_subCap, 0, _subCap.Length);
			requestBlob.WriteCmdSize ();
			
			TpmBlob responseBlob = _tpmProvider.TransmitAndCheck (requestBlob);
			
		}
		
		public override void Clear ()
		{
		}
		
				
		
		
		
		

	}
}
