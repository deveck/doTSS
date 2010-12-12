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

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
{


	public class DotNetCfgPermission : Permission
	{
		private PermissionElement _permissionElement;
		
		public override string Pid 
		{
			get { return _permissionElement.Pid; }
		}
		
		public override string Subsystem 
		{
			get { return _permissionElement.Subsystem; }
		}
		
			
		public DotNetCfgPermission (AccessControlList acl, PermissionElement permissionElement)
		{
			_permissionElement = permissionElement;
			
			foreach(PermissionMember permissionMember in permissionElement)
				_permissionEntries.Add(DotNetCfgPermissionFactory.CreatePermissionEntry(acl, UniqueId, permissionMember));
		}
	}
}
