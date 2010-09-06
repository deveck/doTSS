// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Configuration
{

	/// <summary>
	/// Abstract authentication specific implementation for
	/// Ssh certificate authentication
	/// </summary>
	public abstract class SslAuthenticationParameters : Authentication
	{

		/// <summary>
		/// Gets the associated certificate hash
		/// </summary>
		public abstract string CertificateHash{get;}
		
		
		public SslAuthenticationParameters ()
		{
		}
	}
}
