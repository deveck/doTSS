// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Configuration
{

	/// <summary>
	/// Provides the configuration for a single tpm device
	/// </summary>
	public interface ITPMDeviceConfiguration
	{
		string TPMName { get; }
		string TPMType { get; }
		
		IDictionary<string, string> Parameters{ get; }
		
	}
}
