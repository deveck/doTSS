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
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils;
using Iaik.Utils.Hash;

namespace Iaik.Tc.TPM.Commands
{
    [TPMConsoleCommand("tpm_session_rng")]
    public class TPMSessionRNGCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"tpm_session_rng Args: [local_alias] [number of random values]
    Specify the tpm to use by [local_alias]. These aliases can be defined using the tpm_select command.
	Generates the specified number of random values
    ";
            }
        }

        public override void Execute (string[] commandline)
        {
        	if (commandline.Length < 2)
        	{
        		_console.Out.WriteLine ("Error: [local_alias] not specified");
        		return;
        	}
        	else if (commandline.Length < 3)
        	{
        		_console.Out.WriteLine ("Error: [number of random values] not specified");
        		return;
        	}

            ClientContext ctx = _console.GetValue<ClientContext> ("client_context", null);

            if (ctx == null)
            {
        		_console.Out.WriteLine ("No active connection was found");
        		return;
        	}

			string localAlias = commandline[1];
        	string sNumberOfValues = commandline[2];
   			int numberOfValues = int.Parse(sNumberOfValues);
   	
   			IDictionary<string, TPMSession> tpmSessions = _console.GetValue<IDictionary<string, TPMSession>> ("tpm_sessions", null);
   			if (tpmSessions == null || tpmSessions.ContainsKey (localAlias) == false)
			{
        		_console.Out.WriteLine ("Error: Specified local alias was not found");
        		return;
        	}
        	
        	Random r = tpmSessions[localAlias].CreateRNG();
   			byte[] randomBuffer = new byte[20];
   			for(int i = 0; i<numberOfValues; i++)
   			{
   				r.NextBytes(randomBuffer);
   				_console.Out.WriteLine("#{0}: int: {1} double: {2} bytes: {3}", i+1, r.Next(), r.NextDouble(), ByteHelper.ByteArrayToHexString(randomBuffer));
   			}
   		
        }
    }
}
