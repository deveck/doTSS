// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Configuration;
using Iaik.Tc.TPM.Context;

namespace Iaik.Tc.TPM.Subsystems
{


	/// <summary>
	/// Adds client specific properties to the BaseSubsystem
	/// </summary>
	public abstract class BaseClientSubsystem<TRequest> : BaseSubsystem<TRequest>
	{

		
		
		public BaseClientSubsystem (EndpointContext context)
			: base(context)
		{
		}
		
		 
	}
}
