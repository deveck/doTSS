// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using System.Reflection;
using Iaik.Utils.CommonFactories;

namespace Iaik.Tc.TPM.Keystore
{

	/// <summary>
    /// Helper class for instantiating TPM keystore providers
    /// </summary>
	public static class TPMKeystoreProviders
	{
		/// <summary>
        /// Create a TPM keystore provider using its name.
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TPMKeystoreProvider Create (String providerName, IDictionary<String, String> options)
        {
        	return GenericClassIdentifierFactory.CreateFromClassIdentifierOrType<TPMKeystoreProvider>(providerName, options);
        }

        
	}
}