using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.Tpm.Commands
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
