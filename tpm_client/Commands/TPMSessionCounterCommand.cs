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
    [TPMConsoleCommand("tpm_session_counter")]
    public class TPMSessionCounterCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"tpm_session_counter Args: [local_alias] [counter_subcommand]
    Specify the tpm to use by [local_alias]. These aliases can be defined using the tpm_select command.
	Performs administrative functions.
    Valid values for counter_subcommand: 
    	create    [label_4chars]
    	read	  [counter id]
    	increment [counter_id]
    	release   [counter_id]
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
        		_console.Out.WriteLine ("Error: [counter_subcommand] not specified");
        		return;
        	}

            ClientContext ctx = _console.GetValue<ClientContext> ("client_context", null);

            if (ctx == null)
            {
        		_console.Out.WriteLine ("No active connection was found");
        		return;
        	}

			string localAlias = commandline[1];
        	string counterCommand = commandline[2];
   
			IDictionary<string, TPMSession> tpmSessions = _console.GetValue<IDictionary<string, TPMSession>> ("tpm_sessions", null);
   
			if (tpmSessions == null || tpmSessions.ContainsKey (localAlias) == false)
			{
        		_console.Out.WriteLine ("Error: Specified local alias was not found");
        		return;
        	}
    
			
			
			if (counterCommand == "create")
			{
				ProtectedPasswordStorage ownerAuth;
				if(commandline.Length < 4)
				{
					_console.Out.WriteLine("Error: Could not find label");
					return;
				}

				string sLabel = commandline[3];
				if(sLabel.Length != 4)
				{
					_console.Out.WriteLine("Error: Label needs to be 4 chars long");
					return;
				}

				CounterContext counterCtx = tpmSessions[localAlias].CounterClient.CreateCounter(Encoding.ASCII.GetBytes(sLabel));
				_console.Out.WriteLine("Created new counter with id #{0}", counterCtx.CounterId);
        	}
					
			else if (counterCommand == "read")
			{
				ProtectedPasswordStorage ownerAuth;
				if(commandline.Length < 4)
				{
					_console.Out.WriteLine("Error: Could not find counter id");
					return;
				}
				
				uint counterId = uint.Parse(commandline[3]);
				CounterContext counter = tpmSessions[localAlias].CounterClient.GetCounter(counterId);
				_console.Out.WriteLine("Value of counter #{0}: {1}", counter.CounterId, counter.CounterValue);
			}
			
			else if (counterCommand == "increment")
			{
				ProtectedPasswordStorage ownerAuth;
				if(commandline.Length < 4)
				{
					_console.Out.WriteLine("Error: Could not find counter id");
					return;
				}
				
				uint counterId = uint.Parse(commandline[3]);
				CounterContext counter = tpmSessions[localAlias].CounterClient.GetCounter(counterId);
				_console.Out.WriteLine("Incremented counter, new value of counter #{0}: {1}", counter.CounterId, counter.Increment());
			}
			
			else if (counterCommand == "release")
			{
				ProtectedPasswordStorage ownerAuth;
				if(commandline.Length < 4)
				{
					_console.Out.WriteLine("Error: Could not find counter id");
					return;
				}
				
				uint counterId = uint.Parse(commandline[3]);
				CounterContext counter = tpmSessions[localAlias].CounterClient.GetCounter(counterId);
				counter.Release();
				_console.Out.WriteLine("Released counter #{0}", counter.CounterId);
			}
			else
        		_console.Out.WriteLine ("Error, unknown counter_subcommand '{0}'", counterCommand);
        }
    }
}
