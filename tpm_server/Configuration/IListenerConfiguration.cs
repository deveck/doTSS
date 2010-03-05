// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;

namespace Iaik.Tc.Tpm.Configuration
{


	/// <summary>
	/// Implementet by the configuration implementation, represents a single listener Configuration
	/// </summary>
	public interface IListenerConfiguration
	{
		/// <summary>
		/// Returns the type of the listener
		/// </summary>
		string ListenerType{ get; }
		
		/// <summary>
		/// Returns all defined parameters of this listener
		/// </summary>
		IEnumerable<KeyValuePair<string, string>> Parameters{get;}
		
		/// <summary>
		/// Looks for a named parameter
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		string FindParameter(string name);
	}
}