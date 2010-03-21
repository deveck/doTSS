// 
// 
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;
using Iaik.Tc.TPM.Authentication;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
{


	public class AuthenticationElement : ConfigurationElement, IAuthenticationMethod
	{
		private const string PROP_TYPE = "type";
		
		/// <summary>
		/// Returns 
		/// </summary>
		[ConfigurationProperty(PROP_TYPE, IsRequired=true)]
        public string AuthIdentifier
		{
			get{ return (string)base[PROP_TYPE]; }
		}

        #region IAuthenticationMethod Members

        public AuthenticationMechanismChecker AuthChecker
        {
            get { return new AuthenticationMechanismChecker(this); }
        }

        #endregion
    }
}
