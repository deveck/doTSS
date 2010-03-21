// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
{

	/// <summary>
    /// Dot Net Configuration implementation of the CleartextPasswordAuthentication
    /// </summary>
    [DotNetCfgAuthentication("cleartext_password")]
	public class DotNetCfgCleartextPasswordAuthentication : CleartextPasswordAuthentication
	{
		
		/// <summary>
		/// Permission configuration element
		/// </summary>
		private AuthenticationType _authenticationT;
		
		public override string Password 
		{
			get { return _authenticationT["password"].Value; }
		}
		
		
		public DotNetCfgCleartextPasswordAuthentication (AuthenticationType authenticationType)
		{
			_authenticationT = authenticationType;
			_authenticationType = authenticationType.Type;
		}
	}
}
