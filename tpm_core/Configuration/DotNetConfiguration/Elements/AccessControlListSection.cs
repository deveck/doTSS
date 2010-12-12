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
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements
{

	/// <summary>
	/// Parses the access control list from the configuration file
	/// </summary>
	public class AccessControlListSection : ConfigurationSection
	{

		/// <summary>
		/// group collection tag name
		/// </summary>
		private const string PROP_GROUPS = "groups";
		
		/// <summary>
		/// user collection tag name
		/// </summary>
		private const string PROP_USERS = "users";
		
		/// <summary>
		/// permission collection tag name
		/// </summary>
		private const string PROP_PERMISSIONS = "permissions";
	
		/// <summary>
		/// Returns a collection of all defined groups
		/// </summary>
		[ConfigurationProperty(PROP_GROUPS)]
		public GroupElementCollection Groups
		{
			get{ return (GroupElementCollection)base[PROP_GROUPS];}
		}
		
		/// <summary>
		/// Returns a collection of all defined users
		/// </summary>
		[ConfigurationProperty(PROP_USERS)]
		public UserElementCollection Users
		{
			get{ return (UserElementCollection)base[PROP_USERS];}
		}
		
		/// <summary>
		/// Returns a permission collection with associated users
		/// </summary>
		[ConfigurationProperty(PROP_PERMISSIONS)]
		public PermissionsGroup Permissions
		{
			get{ return (PermissionsGroup)base[PROP_PERMISSIONS]; }
		}
	}
}
