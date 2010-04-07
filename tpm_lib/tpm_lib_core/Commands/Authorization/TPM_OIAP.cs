
using System;

namespace Iaik.Tc.TPM.Library.Commands.Authorization
{


	public class TPM_OIAP : TPMCommand
	{


		public override void Init (Iaik.Tc.TPM.Library.Common.Parameters param, Iaik.Tc.TPM.Lowlevel.TPMProvider tpmProvider)
		{
			base.Init (param, tpmProvider);
			
		}

		
		
		public override void Clear ()
		{
		}

	}
}
