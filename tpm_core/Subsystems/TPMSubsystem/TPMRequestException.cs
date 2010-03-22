// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{


	public class TPMRequestException : Exception
	{

		public TPMRequestException (string message)
			: base(message)
		{
		
		}
		
		public override string ToString ()
		{
			return string.Format ("The TPMRequest failed with the following error: {0}", Message);
		}

	}
}
