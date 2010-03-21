// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Configuration
{

	/// <summary>
	/// Abstract authentication specific implementation for
	/// CleartextPasswordAuthentication, NOT FOR PRODUCTIONAL SYSTEMS
	/// </summary>
	public abstract class CleartextPasswordAuthentication : Authentication
	{

		/// <summary>
		/// Gets the associated password
		/// </summary>
		public abstract string Password{get;}
		
		
		public CleartextPasswordAuthentication ()
		{
		}
	}
}
