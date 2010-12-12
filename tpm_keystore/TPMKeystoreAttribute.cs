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
