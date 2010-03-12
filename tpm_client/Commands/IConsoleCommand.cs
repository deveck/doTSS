using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.Tpm.Commands
{
    /// <summary>
    /// Interface for all commands that can be executed on the tpm console
    /// </summary>
    public interface IConsoleCommand
    {
        /// <summary>
        /// Gets the help text for this command. It should look similar to the following example
        /// 
        /// help        Shows all available commands
        /// 
        /// connect     Args: [connection_method] [connectionmethod arguments]
        ///             Connects to the specified TPM server using 'connection_method'
        /// </summary>
        string HelpText { get; }

        /// <summary>
        /// Initializes the command
        /// </summary>
        /// <param name="console"></param>
        void Initialize(TPMConsole console);

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="commandline">The whole commandline, splitted by argument. The first element is always the name of the command</param>
        void Execute(string[] commandline);
    }
}
