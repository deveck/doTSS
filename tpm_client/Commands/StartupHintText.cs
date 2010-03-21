using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.TPM.Commands
{
    [TPMConsoleStartupCommand()]
    public class StartupHintText: ConsoleCommandBase

    {
        public override string HelpText
        {
            get { return null; }
        }

        public override void Execute(string[] commandline)
        {
            _console.Out.WriteLine("Welcome to the TPM_csharp framework tester\n");
            _console.Out.WriteLine("Type 'help' for more information");
        }
    }
}
