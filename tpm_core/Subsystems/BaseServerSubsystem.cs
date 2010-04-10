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
	/// Adds server specific properties to the BaseSubsystem
	/// </summary>
	public abstract class BaseServerSubsystem<TRequest> : BaseSubsystem<TRequest>
	{

		/// <summary>
		/// Contains the configuration of the framework
		/// </summary>
		protected IConnectionsConfiguration _config;

		internal IConnectionsConfiguration ConnectionsConfig 
		{
			get { return _config; }
		}
		
		public BaseServerSubsystem (EndpointContext context, IConnectionsConfiguration config)
			: base(context)
		{
			_config = config;
		}
	}
}
