using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Commands
{
    [TPMConsoleCommand("tpm_session_pcr")]
    public class TPMSessionPCRCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"tpm_session_pcr Args: [local_alias] [pcr_subcommand]
    Specify the tpm to use by [local_alias]. These aliases can be defined using the tpm_select command.
	
		pcr_report    Reports the status of all platform configuration registers
		";
	
            }
        }

        public override void Execute (string[] commandline)
        {
        	if (commandline.Length < 2)
        		_console.Out.WriteLine ("Error: [local_alias] not specified");
        	else if (commandline.Length < 3)
        		_console.Out.WriteLine ("Error: [pcr_subcommand] not specified");

            ClientContext ctx = _console.GetValue<ClientContext> ("client_context", null);

            if (ctx == null)
            {
        		_console.Out.WriteLine ("No active connection was found");
        		return;
        	}

			string localAlias = commandline[1];
        	string pcrCommand = commandline[2];
   
			IDictionary<string, TPMSession> tpmSessions = _console.GetValue<IDictionary<string, TPMSession>> ("tpm_sessions", null);
   
			if (tpmSessions == null || tpmSessions.ContainsKey (localAlias) == false)
			{
        		_console.Out.WriteLine ("Error: Specified local alias was not found");
        		return;
        	}
    
			
			
			if (pcrCommand == "report")
			{
				uint pcrCount = tpmSessions[localAlias].CapabilityClient.GetPCRCount();
				
				for(uint i = 0; i<pcrCount; i++)
					_console.Out.WriteLine("#{0}: {1}", i, ByteHelper.ByteArrayToHexString(tpmSessions[localAlias].IntegrityClient.PCRValue(i)));
			
        	}
			else
        		_console.Out.WriteLine ("Error, unknown pcr_subcommand '{0}'", commandline[1]);
        }
    }
}
