using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Configuration.DotNetConfiguration.Elements;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration
{
    /// <summary>
    /// Dot Net Configuration implementation of the group class
    /// </summary>
    public class DotNetCfgGroup : Group
    {
        /// <summary>
        /// The GroupElement from the parsed configuration
        /// </summary>
        private GroupElement _groupElement;

        public DotNetCfgGroup(GroupElement groupElement)
        {
            _groupElement = groupElement;
        }

        public override string Gid
        {
            get { return _groupElement.Gid; }
        }

        public override string Name
        {
            get { return _groupElement.Name; }
        }
    }
}
