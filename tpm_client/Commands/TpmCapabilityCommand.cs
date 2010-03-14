using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Context;

namespace Iaik.Tc.Tpm.Commands
{
    [TPMConsoleCommand("tpm_capability")]
    public class TpmCapabilityCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"tpm_capability Args: [cap_type]
    Returns the capability specified by cap_type by one of these values: tpm_version";
            }
        }

        public override void Execute (string[] commandline)
        {
        	if (commandline.Length < 2)
        		_console.Out.WriteLine ("Error: [cap_type] not specified");

            ClientContext ctx = _console.GetValue<ClientContext> ("client_context", null);

            if (ctx == null)
            {
        		_console.Out.WriteLine ("No active connection was found");
        		return;
        	}

			if (commandline[1] == "tpm_version")
			{
        		ctx.TpmClient.Capabilities.GetTPMVersion ();
        	}
			else
        		_console.Out.WriteLine ("Error, unknown cap_type '{0}'", commandline[1]);
        }
    }
}
