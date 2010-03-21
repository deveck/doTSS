// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Lowlevel
{
    /// <summary>
    /// Attribute to flag a class as TPM provider.    
    /// </summary>    
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TPMProviderAttribute : Attribute
    {
        /// <summary>
        /// The name of this provider
        /// </summary>
        private readonly String providerName_;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerName"></param>
        public TPMProviderAttribute(String providerName)
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
