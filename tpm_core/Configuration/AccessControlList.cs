//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.Tpm.Configuration
{
    /// <summary>
    /// Base Access Control List configuration.
    /// </summary>
    public abstract class AccessControlList
    {

        /// <summary>
        /// Gets all defined framework users
        /// 
        /// @key: user id
        /// </summary>
        public abstract IDictionary<string, User> UsersById{ get; }

        /// <summary>
        /// Enumerates though the users
        /// </summary>
        public IEnumerable<User> Users
        {
            get{ return UsersById.Values; }
        }

        /// <summary>
        /// Gets all defined framework groups
        /// </summary>
        public abstract IDictionary<string, Group> GroupsById { get; }

        /// <summary>
        /// Enumerates through the groups
        /// </summary>
        public IEnumerable<Group> Groups
        {
            get { return GroupsById.Values; }
        }

        /// <summary>
        /// Gets all defined permissions with its associated users and groups
        /// </summary>
        public abstract IDictionary<string, Permission> Permissions { get; }

        /// <summary>
        /// Looks for an users with the specified id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual User FindUserById(string userId)
        {
            IDictionary<string, User> users = UsersById;

            if (users.ContainsKey(userId))
                return users[userId];
            else
                return null;
        }
    }
}
