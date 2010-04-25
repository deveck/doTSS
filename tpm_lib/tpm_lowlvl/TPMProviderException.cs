// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Lowlevel
{


	public class TPMProviderException : Exception
	{

		public TPMProviderException (string message)
			:base(message)
		{
		}
	}
}
