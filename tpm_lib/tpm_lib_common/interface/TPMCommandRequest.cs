// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;

namespace Iaik.Tc.Tpm.library.common
{
	/// <summary>
	/// Represents the command request for a specific TPM command, that should
	/// be processed by the TPM library.
	/// </summary>
	public class TPMCommandRequest
	{

		private String name_;
		private Parameters params_;
		public TPMCommandRequest()
		{
		}
		
		public TPMCommandRequest(String commandIdentifier, Parameters param)
		{
			name_ = commandIdentifier;
			params_ = param;
		}
		
		public Parameters Parameters
		{
			get
			{
				return params_;				
			}
			
			set
			{
				if(value == null)
					throw new ArgumentNullException("params_", "TPMCommandRequest params_ must not be null");
				params_ = value;
			}	
			
		}
		
		//public 
	}
}
