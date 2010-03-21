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
