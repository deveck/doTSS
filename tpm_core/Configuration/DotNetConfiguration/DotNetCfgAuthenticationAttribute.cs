// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration
{

	/// <summary>
	/// Defines the string identifier for an authentication type.
	/// Can be queried without creating an instance
	/// </summary>
	public class DotNetCfgAuthenticationAttribute : Attribute
	{
		
		private string _authenticationName;

		public string AuthenticationName
		{
			get{ return _authenticationName;}
		}
		
		public DotNetCfgAuthenticationAttribute (string authenticationName)
		{
			_authenticationName = authenticationName;
		}
	}
}
