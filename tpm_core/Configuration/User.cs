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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;

namespace Iaik.Tc.TPM.Configuration
{
    /// <summary>
    /// Represents an user of the tpm framework
    /// </summary>
    public abstract class User : IPermissionMember
    {
        protected IDictionary<string, Group> _memberOf;

		
		public IdTypeEnum IdType
		{
			get{ return IdTypeEnum.User; }
		}
		
		public string Id
		{
			get{ return Uid;}
		}
		
		public IEnumerable<IPermissionMember> SubPermissionMembers
		{
			get{ return _memberOf.Values.Cast<IPermissionMember>(); }
		}
		
        /// <summary>
        /// Enumerates thoug all associated groups
        /// </summary>
        public IEnumerable<Group> MemberOf
        {
            get { return _memberOf.Values; }
        }

		/// <summary>
		/// Contains all Authentications that are supported
		/// </summary>
		protected IDictionary<string, Authentication> _authentications;
	
		/// <summary>
		/// Enumerates through all associated authentications
		/// </summary>
		public virtual IEnumerable<Authentication> Authentications
		{
			get{ return _authentications.Values; }
		}
		
		/// <summary>
		/// Returns the user specific settings fo the specified authentication
		/// </summary>
		/// <param name="authName"></param>
		/// <returns></returns>
		public virtual Authentication GetAuthentication(string authName)
		{
			if(SupportsAuthentication(authName))
				return _authentications[authName];
			else
				return null;
		}
		
        /// <summary>
        /// Gets the unique id of the user
        /// </summary>
        public abstract string Uid { get; }

        /// <summary>
        /// Gets the name of the user
        /// </summary>
        public abstract string Name { get; }

		/// <summary>
		/// Checks if the user is a member of group
		/// </summary>
		/// <param name="group">The group to check</param>
		/// <returns>
		/// </returns>
        public virtual bool IsMemberOf(Group group)
        {
            return _memberOf.ContainsKey(group.Gid);
        }
		
		/// <summary>
		/// Checks if the authentication method with the specified name is supported by this user
		/// </summary>
		/// <param name="name">Name to check</param>
		/// <returns>
		/// </returns>
		public virtual bool SupportsAuthentication(string name)
		{
			return _authentications.ContainsKey(name);
		}

		/// <summary>
		/// Checks if the specified authentication type is supported by this user
		/// </summary>
		/// <param name="type">type to check</param>
		/// <returns>
		/// </returns>
		public virtual bool SupportsAuthentication(Type type)
		{
			foreach(Authentication auth in Authentications)
			{
				if(type.IsAssignableFrom(auth.GetType()))
					return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// Checks if the specified authentication method is supported by this user
		/// </summary>
		/// <returns>
		/// </returns>
		public virtual bool SupportsAuthentication<T>() where T: Authentication
		{
			return SupportsAuthentication(typeof(T));
		}
		
        public override bool Equals(object obj)
        {
            if (obj is User)
                return (obj as User).Uid.Equals(Uid);

            return false;
        }

        public override int GetHashCode()
        {
            return Uid.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Uid, Name);
        }

    }
}
