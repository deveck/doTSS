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
