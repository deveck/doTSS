using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Context;

namespace Iaik.Tc.TPM.Commands
{
    [TPMConsoleCommand("tpm_list")]
    public class TpmListDevicesCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"tpm_list 
    Returns a list of all usable tpm devices by the current connection and user";
            }
        }

        public override void Execute (string[] commandline)
        {
        	ClientContext ctx = _console.GetValue<ClientContext> ("client_context", null);

            if (ctx == null)
            {
        		_console.Out.WriteLine ("No active connection was found");
        		return;
        	}

			_console.Out.WriteLine ("Tpm devices:");
        	foreach (string tpmDevice in ctx.TpmClient.TpmDevices)
			{
        		_console.Out.WriteLine ("\t" + tpmDevice);
			}
			
        }
    }
}
