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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;
using System.Configuration;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
{
    public class DotNetCfgAccessControlList : AccessControlList
    {
        /// <summary>
        /// Configuration section
        /// </summary>
        private AccessControlListSection _accessControlListSection;

        public DotNetCfgAccessControlList()
            :this((AccessControlListSection)ConfigurationManager.GetSection("accessControlList"))
        {
        }

        public DotNetCfgAccessControlList(AccessControlListSection accessControlListSection)
        {
            _accessControlListSection = accessControlListSection;
			
			_groups = new Dictionary<string, Group>();
			foreach(GroupElement groupElement in _accessControlListSection.Groups)
			{
				Group group = new DotNetCfgGroup(groupElement);
				
				if(_groups.ContainsKey(group.Gid))
					throw new ConfigurationErrorsException(
					      string.Format("Group with id '{0}' is defined more than once", group.Gid));
				_groups.Add(group.Gid, group);
			}
			
			_users = new Dictionary<string, User>();
			foreach(UserElement userElement in _accessControlListSection.Users)
			{
				User user = new DotNetCfgUser(this, userElement);
				
				if(_users.ContainsKey(user.Uid))
					throw new ConfigurationErrorsException(
					      string.Format("User with od '{0}' is defined more than once", user.Uid));
				
				_users.Add(user.Uid, user);
			}
			
			_permissions = new Dictionary<string, Permission>();
			foreach(PermissionElement permissionElement in _accessControlListSection.Permissions)
			{
				Permission permission = new DotNetCfgPermission(this, permissionElement);
				
				if(_permissions.ContainsKey(permission.UniqueId))
					throw new ConfigurationErrorsException(string.Format("Permission '{0}' is defined more than once", permission.UniqueId));
				
				_permissions.Add(permission.UniqueId, permission);
			}
        }

        
    }
}
