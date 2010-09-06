// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
{

	/// <summary>
    /// Dot Net Configuration implementation of the SshAuthentication
    /// </summary>
    [DotNetCfgAuthentication("ssl_auth")]
	public class DotNetCfgSslAuthentication : SslAuthenticationParameters
	{
		
		/// <summary>
		/// Permission configuration element
		/// </summary>
		private AuthenticationType _authenticationT;
		
		public override string CertificateHash
		{
			get 
			{				
				//FIXME: parameter retrieval by name does not work
				foreach(AuthenticationParameter par in _authenticationT)
				{
					if(par.Name == "cert_hash")
						return par.Value;
				}
				
				return "";
			}
		}
		
		
		public DotNetCfgSslAuthentication (AuthenticationType authenticationType)
		{
			_authenticationT = authenticationType;
			_authenticationType = authenticationType.Type;
		}
	}
}
