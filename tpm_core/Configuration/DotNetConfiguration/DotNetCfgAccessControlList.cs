using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Configuration.DotNetConfiguration.Elements;
using System.Configuration;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration
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
        }

        public override IDictionary<string, User> UsersById
        {
            get { throw new NotImplementedException(); }
        }

        public override IDictionary<string, Group> GroupsById
        {
            get { throw new NotImplementedException(); }
        }

        public override IDictionary<string, Permission> Permissions
        {
            get { throw new NotImplementedException(); }
        }
    }
}
