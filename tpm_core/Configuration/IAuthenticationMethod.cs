// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Authentication;

namespace Iaik.Tc.TPM.Configuration
{

	/// <summary>
	/// Configuration of an authentication method
	/// </summary>
	public interface IAuthenticationMethod
	{
		/// <summary>
		/// Returns the identifier or type name of the authentication method
		/// </summary>
		string AuthIdentifier{ get; }


        AuthenticationMechanismChecker AuthChecker { get; }
	}
	
}
