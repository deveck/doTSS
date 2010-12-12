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

namespace Iaik.Tc.TPM.Commands
{
    /// <summary>
    /// Must-Have attributes for all TPM console commands
    /// </summary>
    public class TPMConsoleCommandAttribute : Attribute
    {
        private string[] _cmdNames;

        public string[] CmdNames
        {
            get { return _cmdNames; }
        }


        public TPMConsoleCommandAttribute(params string[] cmdNames)
        {
            _cmdNames = cmdNames;
        }

        public bool ContainsName(string cmdName)
        {
            foreach (string myCmdName in _cmdNames)
            {
                if (myCmdName.Equals(cmdName))
                    return true;

            }
            return false;

        }

    }
}
