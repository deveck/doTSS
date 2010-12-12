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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
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
