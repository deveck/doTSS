// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.CommonAttributes;

namespace Iaik.Tc.TPM.Keystore
{

	/// <summary>
    /// Attribute to flag a class as TPM Keystore provider.
    /// </summary>    
    [AttributeUsage(AttributeTargets.Class)]
	public sealed class TPMKeystoreAttribute : ClassIdentifierAttribute
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerName"></param>
        public TPMKeystoreAttribute(String providerName)
        	: base(providerName)
        {
        }

        /// <summary>
        /// Returns the name of this provider
        /// </summary>
        public String ProviderName
        {
            get{ return base.Identifier; }
        }
	}
}
