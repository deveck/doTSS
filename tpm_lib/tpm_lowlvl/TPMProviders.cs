/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Reflection;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Lowlevel
{
    /// <summary>
    /// Helper class for instantiating TPM providers
    /// </summary>
    public static class TPMProviders
    {
	/// <summary>
	/// The global repository of known TPM provider types.
	/// </summary>
	private static readonly IDictionary<String, Type> providers_ = new Dictionary<String, Type>();
	
	/// <summary>
	/// Static constructor for the TpmProviders class.
	///
	/// This constructor automatically registers all TPM providers
	/// found in the assembly definining this class.
	/// </summary>fac
	static TPMProviders()
	{
           RegisterProviders(typeof(TPMProviders).Assembly);
	}

        /// <summary>
        /// Type filter to extract class types with the "TpmProvider" attribute.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private static bool TpmProviderFilter(Type m, Object criteria)
        {
           return Attribute.IsDefined(m, typeof(TPMProviderAttribute)) &&
                  !m.IsAbstract &&
                  typeof(TPMProvider).IsAssignableFrom(m);
        }

        /// <summary>
        /// Register all provider types from a given assembly.
        /// </summary>
        /// <returns></returns>
        public static void RegisterProviders(Assembly assy)
        {
	    lock(providers_)
            {
                foreach (Module module in assy.GetModules())
                {
                    foreach (Type provider in module.FindTypes(TpmProviderFilter, null))
                    {
                        TPMProviderAttribute pattr = (TPMProviderAttribute)Attribute.GetCustomAttribute(provider, typeof(TPMProviderAttribute));
			if (!providers_.Keys.Contains(pattr.ProviderName))
                        	providers_.Add(pattr.ProviderName, provider);
                    }
                }
            }
        }

        /// <summary>
        /// Create a TPM provider using its name.
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TPMProvider Create (String providerName, IDictionary<String, String> options)
        {
        	Type provider_type;
	    	
			lock(providers_)
              provider_type = providers_[providerName];
            
	    	return Create(provider_type, options);
        }

        /// <summary>
        /// Create a TPM provider of the given a type.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TPMProvider Create(Type provider, IDictionary<String, String> options)
        {
            if (provider.IsAbstract)
                throw new ArgumentException("Can't create abstract TPM providers");

            if (!typeof(TPMProvider).IsAssignableFrom(provider))
                throw new ArgumentException("TPM provider must inherit from iaik.tc.tpm.TPM");

            ConstructorInfo ctor;
            
            // First try the specific constructor
            ctor = provider.GetConstructor(new Type[] { typeof(IDictionary<String, String>) });
            if (ctor != null)
                return (TPMProvider)ctor.Invoke(new object[] { options });

            // Resort to the standard constructor
            ctor = provider.GetConstructor(new Type[] { });
            if (ctor != null)
                return (TPMProvider)ctor.Invoke(new object[] { });

            throw new ArgumentException("Can not find a suitable constructor for this TPM provider");
        }
    }
}
