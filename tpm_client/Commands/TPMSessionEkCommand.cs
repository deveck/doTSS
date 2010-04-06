using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Commands
{
    [TPMConsoleCommand("tpm_session_ek")]
    public class TPMSessionEkCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"tpm_session_ek Args: [local_alias] [ek_subcommand]
    Specify the tpm to use by [local_alias]. These aliases can be defined using the tpm_select command.
	Performs the endorsement key action specified by ek_subcommand.
    Valid values for ek_subcommand: read_pubek";
            }
        }

        public override void Execute (string[] commandline)
        {
        	if (commandline.Length < 2)
        		_console.Out.WriteLine ("Error: [local_alias] not specified");
        	else if (commandline.Length < 3)
        		_console.Out.WriteLine ("Error: [ek_subcommand] not specified");

            ClientContext ctx = _console.GetValue<ClientContext> ("client_context", null);

            if (ctx == null)
            {
        		_console.Out.WriteLine ("No active connection was found");
        		return;
        	}

			string localAlias = commandline[1];
        	string ekCommand = commandline[2];
   
			IDictionary<string, TPMSession> tpmSessions = _console.GetValue<IDictionary<string, TPMSession>> ("tpm_sessions", null);
   
			if (tpmSessions == null || tpmSessions.ContainsKey (localAlias) == false)
			{
        		_console.Out.WriteLine ("Error: Specified local alias was not found");
        		return;
        	}
    
			
			
			if (ekCommand == "read_pubek")
			{
        		TPMPubkey pubkey = tpmSessions[localAlias].EndorsementKeyHandling.PublicKey;
        		_console.Out.WriteLine (pubkey.ToString());
        	}
			else
        		_console.Out.WriteLine ("Error, unknown ek_subcommand '{0}'", commandline[1]);
        }
    }
}
