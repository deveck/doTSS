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
