// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;

namespace Iaik.Tc.TPM.Configuration
{
    /// <summary>
    /// Represents a group of the framework
    /// </summary>
    public abstract class Group : IPermissionMember
    {
		public IdTypeEnum IdType
		{
			get{ return IdTypeEnum.Group; }
		}
		
		public bool IsInternal
		{
			get{ return true;}
		}
		
		public string Id
		{
			get{ return Gid; }
		}
		
		public IEnumerable<IPermissionMember> SubPermissionMembers
		{
			get{ return new List<IPermissionMember>(); }
		}
		
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
