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

	/// <summary>
	/// Represents an external (os dependent) user of the framework
	/// </summary>
	public class ExternalUser : IPermissionMember
	{
		/// <summary>
		/// User identifier
		/// </summary>
		protected string _userId;
		
		/// <summary>
		/// Gets the user id of the external user
		/// </summary>
		public string UId
		{
			get{ return _userId;}
		}
		
		/// <summary>
		/// Group identifier
		/// </summary>
		protected ExternalGroup _group;
		
		/// <summary>
		/// Gets the group id of the external user
		/// </summary>
		public ExternalGroup Group
		{
			get{ return _group;}
		}
		
		
		public ExternalUser (string uid, string gid)
		{
			_userId = uid;
			_group = new ExternalGroup(gid);
		}
		
			
		public IdTypeEnum IdType
		{
			get{ return IdTypeEnum.UserExtern; }
		}
		
		public string Id
		{
			get{ return UId; }
		}
		
		public IEnumerable<IPermissionMember> SubPermissionMembers
		{
			get{ return new IPermissionMember[]{_group}; }
		}
		
	}
}
