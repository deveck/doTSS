// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.Tpm.lowlevel.exceptions
{


	public class TpmLowLvlException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public TpmLowLvlException() : this("TPM Backend Error")
		{
		}
		
		public TpmLowLvlException(uint result) : this("TPM Backend Error", result)
		{
		}
			
		
		public TpmLowLvlException(string message) : this(message, 0)
		{			
		}
		
		public TpmLowLvlException(string message, uint result) : base(message)
		{
			
		}
	}
}
