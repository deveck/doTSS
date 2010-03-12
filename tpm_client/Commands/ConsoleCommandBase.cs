using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.Tpm.Commands
{
    public abstract class ConsoleCommandBase : IConsoleCommand
    {
        protected TPMConsole _console;

        #region IConsoleCommand Members

        public abstract string HelpText{get;}

        public void Initialize(TPMConsole console)
        {
            _console = console;
        }

        public abstract void Execute(string[] commandline);

        #endregion
    }
}
