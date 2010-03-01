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
    /// Represents an user of the tpm framework
    /// </summary>
    public abstract class User
    {
        protected IDictionary<string, Group> _memberOf;

        /// <summary>
        /// Gets the list of associated groups
        /// </summary>
        public IDictionary<string, Group> MemberOf
        {
            get { return _memberOf; }
        }

        /// <summary>
        /// Gets the id of the user
        /// </summary>
        public abstract string Uid { get; }

        /// <summary>
        /// Gets the name of the user
        /// </summary>
        public abstract string Name { get; }

        public virtual bool IsMemberOf(Group group)
        {
            return MemberOf.ContainsKey(group.Gid);
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
