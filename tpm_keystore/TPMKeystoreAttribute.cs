// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Keystore
{

	/// <summary>
    /// Attribute to flag a class as TPM Keystore provider.
    /// </summary>    
    [AttributeUsage(AttributeTargets.Class)]
	public sealed class TPMKeystoreAttribute : System.Attribute
	{

		/// <summary>
        /// The name of this provider
        /// </summary>
        private readonly String providerName_;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerName"></param>
        public TPMKeystoreAttribute(String providerName)
        {
            providerName_ = providerName;
        }

        /// <summary>
        /// Returns the name of this provider
        /// </summary>
        public String ProviderName
        {
            get
            {
                return providerName_;
            }
        }
	}
}
