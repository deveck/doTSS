// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem.Authorization
{

	public class AuthHandle
	{

		public enum AuthType
		{
			OIAP,
			OSAP
		}
		
		private AuthType _authType;
		
		public AuthHandle (AuthType authType)
		{
			_authType = authType;
		}
	}
}
