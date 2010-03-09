// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.library;
using Iaik.Tc.Tpm.library.common;

namespace Iaik.Tc.Tpm.library.commands
{
	[TpmCommands(Iaik.Tc.Tpm.library.common.TPMCommandNames.TPM_CMD_GetCapability)]
	public sealed class TPM_GetCapability : TpmCommand
	{
		public override void Process ()
		{
			throw new System.NotImplementedException ();
		}
		
		public override void Clear ()
		{
			throw new System.NotImplementedException();
		}
		
		public override void Process (Parameters param)
		{
			throw new System.NotImplementedException();
		}
		
		public override Iaik.Tc.Tpm.lowlevel.data.TpmBlob ToBlob ()
		{
			throw new System.NotImplementedException();
		}
		
		
		
		
		

	}
}
