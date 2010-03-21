using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.TPM.Commands
{

    [TPMConsoleCommand("help")]
    public class HelpCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get { return "help\tShows this command summary"; }
        }

        public override void Execute(string[] commandline)
        {
            _console.Out.WriteLine("\nCommand summary:\n");

            foreach (KeyValuePair<string, IConsoleCommand> cmd in _console.Commands)
            {
                _console.Out.WriteLine(cmd.Value.HelpText);
                _console.Out.WriteLine();
            }
        }
    }
}
