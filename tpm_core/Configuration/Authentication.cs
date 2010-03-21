// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Configuration
{

	/// <summary>
	/// Abstract implementation of an authentication associated with a virtual user.
	/// The parameters are parsed in the implementations
	/// </summary>
	public abstract class Authentication
	{
		
		protected string _authenticationType;

		/// <summary>
		/// Gets the string identifier for this Authentication entry
		/// </summary>
		public virtual string AuthenticationType
		{
			get{ return _authenticationType; }
		}
		
		public Authentication ()
		{
		}
	}
}
