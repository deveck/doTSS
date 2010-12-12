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

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
{

	/// <summary>
	/// Defines the string identifier for an authentication type.
	/// Can be queried without creating an instance
	/// </summary>
	public class DotNetCfgAuthenticationAttribute : Attribute
	{
		
		private string _authenticationName;

		public string AuthenticationName
		{
			get{ return _authenticationName;}
		}
		
		public DotNetCfgAuthenticationAttribute (string authenticationName)
		{
			_authenticationName = authenticationName;
		}
	}
}
