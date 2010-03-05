// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;

namespace Iaik.Tc.Tpm.Configuration
{

	/// <summary>
	/// Defines connection related configuration options
	/// </summary>
	public interface IConnectionsConfiguration
	{
		/// <summary>
		/// Returns all defined listeners with their configurations
		/// </summary>
		IEnumerable<IListenerConfiguration> Listeners{get;}
	}
}
