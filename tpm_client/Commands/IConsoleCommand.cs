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
