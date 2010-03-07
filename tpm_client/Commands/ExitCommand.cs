using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.Tpm.Commands
{
    [TPMConsoleCommand("exit")]
    public class ExitCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get { return "exit\tExits the framework test program"; }
        }

        public override void Execute(string[] commandline)
        {
            _console.RunConsoleLoop = false;
        }
    }
}
