using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.Tpm.Configuration
{
    /// <summary>
    /// Represents a group of the framework
    /// </summary>
    public abstract class Group
    {
        /// <summary>
        /// Gets the Id of the group
        /// </summary>
        public abstract string Gid{get;}

        /// <summary>
        /// Gets the Name of the group
        /// </summary>
        public abstract string Name{get;}

        public override bool Equals(object obj)
        {
            if (obj is Group)
                return (obj as Group).Gid.Equals(Gid);

            return false;
        }

        public override int GetHashCode()
        {
            return Gid.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Gid, Name);
        }

    }
}
