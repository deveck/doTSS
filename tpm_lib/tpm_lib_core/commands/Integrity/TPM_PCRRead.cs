// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Runtime.InteropServices;
using Iaik.Tc.Tpm.library;
using Iaik.Tc.Tpm.library.common;
using Iaik.Tc.Tpm.lowlevel;



namespace Iaik.Tc.Tpm.library.commands
{
	[TpmCommands(TPMCommandNames.TPM_CMD_PCRRead)]
	public sealed class TPM_PCRRead : TpmCommand
	{
		private UInt32 register_ = 0;
		public override void Clear ()
		{
			throw new System.NotImplementedException();
		}
		
		
		
		public TPM_PCRRead() : base(TpmCmdTags.TPM_TAG_RQU_COMMAND, TpmOrdinals.TPM_ORD_PcrRead)
		{	
		}
	
		public override void Init (Parameters param, TPMProvider provider)
		{
			base.Init(param, provider);
			throw new System.NotImplementedException ();
		}

		public override void Process ()
		{
			throw new System.NotImplementedException();
		}
	
		
	}
}
