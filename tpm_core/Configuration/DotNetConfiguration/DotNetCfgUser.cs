using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Configuration.DotNetConfiguration.Elements;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration
{
    /// <summary>
    /// Dot Net Configuration implementation of the user class
    /// </summary>
    public class DotNetCfgUser : User
    {
        /// <summary>
        /// the UserElement from the parsed configuration
        /// </summary>
        private UserElement _userElement;

        public DotNetCfgUser(UserElement userElement)
        {
            _userElement = userElement;
        }

        public override string Uid
        {
            get { return _userElement.Uid; }
        }

        public override string Name
        {
            get { return _userElement.Name; }
        }
    }
}
