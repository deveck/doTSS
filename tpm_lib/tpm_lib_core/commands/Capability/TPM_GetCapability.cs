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
		
		public override void Init (Parameters param, TPMProvider tpmProvider)
		{
			base.Init (param, tpmProvider);
			
			_capArea = param.GetValueOf<CapabilityData.TPMCapabilityArea> ("capArea");
			_subCap = param.GetValueOf<byte[]> ("subCap");
		}
		
		public override TPMCommandResponse Process ()
		{
			TPMBlob requestBlob = new TPMBlob ();
			requestBlob.WriteCmdHeader (TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_GetCapability);
			requestBlob.WriteUInt32 ((uint)_capArea);
			requestBlob.WriteUInt32 ((uint)_subCap.Length);
			requestBlob.Write (_subCap, 0, _subCap.Length);
			requestBlob.WriteCmdSize ();
			
			TPMBlob responseBlob = _tpmProvider.TransmitAndCheck (requestBlob);
			//TODO: implement!
			return null;
		}
		
		public override void Clear ()
		{
		}
		
				
		
		
		
		

	}
}
