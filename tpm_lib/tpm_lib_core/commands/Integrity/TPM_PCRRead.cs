// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Runtime.InteropServices;
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;



namespace Iaik.Tc.TPM.Library.Commands
{
	[TPMCommands(TPMCommandNames.TPM_CMD_PCRRead)]
	public sealed class TPM_PCRRead : TPMCommand
	{
		/// <summary>
		/// The PCR register that should be read
		/// </summary>
		private UInt32 _register = 0;
		
		public override void Clear ()
		{
			throw new System.NotImplementedException();
		}
		
		
		
		public TPM_PCRRead() : base(TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_PcrRead)
		{	
		}
	
		public override void Init (Parameters param, TPMProvider provider)
		{
			base.Init(param, provider);
			_register = param.GetValueOf<UInt32>("pcrnum");
			
		//	throw new System.NotImplementedException ();
		}

		public override void Process ()
		{
			TPMBlob requestBlob = new TPMBlob ();
			requestBlob.WriteCmdHeader (TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_PcrRead);
			requestBlob.WriteUInt32 ((uint)_register);
			//requestBlob.WriteUInt32 ((uint)_subCap.Length);
			//requestBlob.Write (_subCap, 0, _subCap.Length);
			requestBlob.WriteCmdSize ();
			
			TPMBlob responseBlob = _tpmProvider.TransmitAndCheck (requestBlob);
			//throw new System.NotImplementedException();
		}
	
		
	}
}
