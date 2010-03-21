// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Lowlevel.Exceptions
{


	public class TPMLowLvlException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public TPMLowLvlException() : this("TPM Backend Error")
		{
		}
		
		public TPMLowLvlException(uint result) : this("TPM Backend Error", result)
		{
		}
			
		
		public TPMLowLvlException(string message) : this(message, 0)
		{			
		}
		
		public TPMLowLvlException(string message, uint result) : base(message)
		{
			
		}
	}
}
