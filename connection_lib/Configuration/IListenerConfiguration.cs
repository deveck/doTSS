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
using System.Collections.Generic;

namespace Iaik.Connection.Configuration
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