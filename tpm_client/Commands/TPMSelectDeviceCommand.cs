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
    [TPMConsoleCommand("tpm_select")]
    public class TPMSelectDeviceCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"tpm_select Args: [remote_identifier] [local_alias] 
    Select the tpm device specified by [remote_identifier] (get list of available devices from tpm_list) and assign a local_alias for use";
            }
        }

        public override void Execute (string[] commandline)
        {
        	if (commandline.Length < 2)
			{
        		_console.Out.WriteLine ("Error: [remote_identifier] not specified");
        		return;
        	}
			else if (commandline.Length < 3)
			{
        		_console.Out.WriteLine ("Error: [local_alias] not specified");
        		return;
        	}
     
        	ClientContext ctx = _console.GetValue<ClientContext> ("client_context", null);

            if (ctx == null)
            {
        		_console.Out.WriteLine ("No active connection was found");
        		return;
        	}
   
			string remoteIdentifier = commandline[1];
        	string localAlias = commandline[2];
   
			IDictionary<string, TPMSession> tpmSessions = _console.GetValue<IDictionary<string, TPMSession>> ("tpm_sessions", null);
        	if (tpmSessions == null)
			{
        		tpmSessions = new Dictionary<string, TPMSession> ();
        		_console.SetValue ("tpm_sessions", tpmSessions);
        	}
   
			if (tpmSessions.ContainsKey (localAlias))
			{
        		_console.Out.WriteLine ("Error: [local_alias] is already defined for a tpm session");
        		return;
        	}

			TPMSession session = ctx.TPMClient.SelectTPMDevice (remoteIdentifier);
        	tpmSessions.Add (localAlias, session);
			
			
        }
    }
}
