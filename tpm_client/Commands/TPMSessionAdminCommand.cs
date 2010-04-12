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
    [TPMConsoleCommand("tpm_session_admin")]
    public class TPMSessionAdminCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"tpm_session_admin Args: [local_alias] [admin_subcommand]
    Specify the tpm to use by [local_alias]. These aliases can be defined using the tpm_select command.
	Performs administrative functions.
    Valid values for admin_subcommand: take_ownership";
            }
        }

        public override void Execute (string[] commandline)
        {
        	if (commandline.Length < 2)
        		_console.Out.WriteLine ("Error: [local_alias] not specified");
        	else if (commandline.Length < 3)
        		_console.Out.WriteLine ("Error: [admin_subcommand] not specified");

            ClientContext ctx = _console.GetValue<ClientContext> ("client_context", null);

            if (ctx == null)
            {
        		_console.Out.WriteLine ("No active connection was found");
        		return;
        	}

			string localAlias = commandline[1];
        	string adminCommand = commandline[2];
   
			IDictionary<string, TPMSession> tpmSessions = _console.GetValue<IDictionary<string, TPMSession>> ("tpm_sessions", null);
   
			if (tpmSessions == null || tpmSessions.ContainsKey (localAlias) == false)
			{
        		_console.Out.WriteLine ("Error: Specified local alias was not found");
        		return;
        	}
    
			
			
			if (adminCommand == "take_ownership")
			{
        		ProtectedPasswordStorage ownerAuth = Utils.ReadPassword ("Enter new owner password:", _console);
        		
				ProtectedPasswordStorage srkAuth = null;
    
				if (ownerAuth != null)
        			srkAuth = Utils.ReadPassword ("Enter new srk password:", _console);
    
				if (ownerAuth == null || srkAuth == null)
				{
        			_console.Out.WriteLine ("Request aborted");
        			return;
        		}
    
				ownerAuth.Hash ();
        		srkAuth.Hash ();
				tpmSessions[localAlias].AdministrationClient.TakeOwnership (ownerAuth, srkAuth);

        	}
			else
        		_console.Out.WriteLine ("Error, unknown admin_subcommand '{0}'", commandline[1]);
        }
    }
}
