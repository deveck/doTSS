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
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Configuration
{


	public interface IPermissionMember
	{
		/// <summary>
		/// Gets the type of the corresponding permission entries
		/// </summary>
		IdTypeEnum IdType{get;}
		
		/// <summary>
		/// Gets the Id of the entry
		/// </summary>
		string Id{get;}
		
		/// <summary>
		/// Gets the sub permission (e.g. for user objects it returns the groups)
		/// </summary>
		IEnumerable<IPermissionMember> SubPermissionMembers{get;}
	}
}
