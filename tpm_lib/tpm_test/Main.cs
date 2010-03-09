// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
using System;
using Iaik.Tc.Tpm.library;

namespace tpm_test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			TPM tpm = new TPM();
			
			tpm.init("linux/device");
			
			Console.WriteLine ("Hello World!");
		}
	}
}
