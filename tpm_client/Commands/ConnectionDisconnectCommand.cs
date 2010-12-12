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
using Iaik.Tc.TPM.Context;

namespace Iaik.Tc.TPM.Commands
{
    [TPMConsoleCommand("connection_disconnect")]
    public class ConnectionDisconnectCommand:ConsoleCommandBase
    {
        public override string HelpText
        {
            get { return @"connection_disconnect Disconnects the currently active connection"; }
        }

        public override void Execute(string[] commandline)
        {
            ClientContext ctx = _console.GetValue<ClientContext>("client_context", null);

            if (ctx == null)
                _console.Out.WriteLine("No active connection was found");
            else
            {
                ctx.Connection.Dispose();
                _console.SetValue("client_context", null);
            }

        }
    }
}
