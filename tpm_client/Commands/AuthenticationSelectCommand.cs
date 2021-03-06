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
    [TPMConsoleCommand("auth_select")]
    public class AuthenticationSelectCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"auth_select Args: [auth_name]
    Selects the authentication identified by auth_name";
            }
        }

        public override void Execute(string[] commandline)
        {
            if (commandline.Length < 2)
                _console.Out.WriteLine("Error: [auth_name] not specified");

            ClientContext ctx = _console.GetValue<ClientContext>("client_context", null);

            if (ctx == null)
            {
                _console.Out.WriteLine("No active connection was found");
                return;
            }

            IStatusIndicator status = ctx.AuthClient.SelectAuthentication(commandline[1]);

            if (status.Succeeded == false)
                _console.Out.WriteLine("Error selecting authentication method: {0}", status.ErrorText);
        }
    }
}
