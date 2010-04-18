// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Iaik.Tc.TPM.Keystore
{

	/// <summary>
    /// Helper class for instantiating TPM keystore providers
    /// </summary>
	public static class TPMKeystoreProviders
	{
		/// <summary>
		/// The global repository of known TPM keystore provider types.
		/// </summary>
		private static readonly IDictionary<String, Type> providers_ = new Dictionary<String, Type>();

		/// <summary>
		/// Static constructor for the TPMKeystoreProviders class.
		///
		/// This constructor automatically registers all TPM keystore providers
		/// found in the assembly definining this class.
		/// </summary>
		static TPMKeystoreProviders ()
		{
			RegisterProviders(typeof(TPMKeystoreProviders).Assembly);
		}
		
		/// <summary>
        /// Type filter to extract class types with the "TpmProvider" attribute.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private static bool TpmProviderFilter(Type m, Object criteria)
        {
           return Attribute.IsDefined(m, typeof(TPMKeystoreAttribute)) &&
                  !m.IsAbstract &&
                  typeof(TPMKeystore).IsAssignableFrom(m);
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
                        TPMKeystoreAttribute pattr = (TPMKeystoreAttribute)Attribute.GetCustomAttribute(provider, typeof(TPMKeystoreAttribute));
						if (!providers_.Keys.Contains(pattr.ProviderName))
							providers_.Add(pattr.ProviderName, provider);
                    }
                }
            }
        }
		
		/// <summary>
        /// Create a TPM keystore provider using its name.
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TPMKeystoreProvider Create (String providerName, IDictionary<String, String> options)
        {
        	Type provider_type;
	    	
			lock(providers_)
              provider_type = providers_[providerName];
            
	    	return Create(provider_type, options);
        }

        /// <summary>
        /// Create a TPM keystore provider of the given a type.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TPMKeystoreProvider Create(Type provider, IDictionary<String, String> options)
        {
            if (provider.IsAbstract)
                throw new ArgumentException("Can't create abstract TPM providers");

            if (!typeof(TPMKeystore).IsAssignableFrom(provider))
                throw new ArgumentException("TPM provider must inherit from iaik.tc.tpm.TPM");

            ConstructorInfo ctor;
            
            // First try the specific constructor
            ctor = provider.GetConstructor(new Type[] { typeof(IDictionary<String, String>) });
            if (ctor != null)
                return (TPMKeystoreProvider)ctor.Invoke(new object[] { options });

            // Resort to the standard constructor
            ctor = provider.GetConstructor(new Type[] { });
            if (ctor != null)
                return (TPMKeystoreProvider)ctor.Invoke(new object[] { });

            throw new ArgumentException("Can not find a suitable constructor for this TPM provider");
        }
	}
}