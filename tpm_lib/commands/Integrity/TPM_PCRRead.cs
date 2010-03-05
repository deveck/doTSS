// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Runtime.InteropServices;
using Iaik.Tc.Tpm.library;

namespace Iaik.Tc.Tpm.library.commands
{
	[TPMCommands("TPM_PCRRead")]
	public sealed class TPM_PCRRead : TpmCommand
	{
		private UInt32 register_ = 0;
		
		public TPM_PCRRead() : base(TpmCmdTags.TPM_TAG_RQU_COMMAND, TpmOrdinals.TPM_ORD_PcrRead)
		{
			
		}
	}
}
