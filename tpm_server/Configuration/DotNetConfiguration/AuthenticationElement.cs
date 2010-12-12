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


// 
// 
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;
using Iaik.Tc.TPM.Authentication;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
{

	[ConfigurationCollection(typeof(Parameter),
	                         AddItemName="add",
	                         RemoveItemName="remove",
	                         ClearItemsName="clear")]
	public class AuthenticationElement : ConfigurationElementCollection, IAuthenticationMethod
	{
		private const string PROP_TYPE = "type";
		
		
		protected override ConfigurationElement CreateNewElement ()
		{
			return new Parameter();
		}

		protected override object GetElementKey (ConfigurationElement element)
		{
			return ((Parameter)element).Name;
		}
		
		protected override string ElementName 
		{
			get { return "add"; }
		}


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
		
		public bool IsDefined(string name)
		{
			foreach(Parameter p in this)
			{
				if(p.Name.Equals(name))
					return true;
			}
			
			return false;
		}

		public string GetValue(string name)
		{
			foreach(Parameter p in this)
			{
				if(p.Name.Equals(name))
					return p.Value;
			}
			
			throw new KeyNotFoundException(string.Format("Key '{0}' not found", name));
		}
        #endregion
    }
}
