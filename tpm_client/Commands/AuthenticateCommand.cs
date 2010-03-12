using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Context;

namespace Iaik.Tc.Tpm.Commands
{
    [TPMConsoleCommand("auth_do")]
    public class AuthenticateCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"auth_do Starts the authentication process";
            }
        }

        public override void Execute(string[] commandline)
        {
            ClientContext ctx = _console.GetValue<ClientContext>("client_context", null);

            if (ctx == null)
            {
                _console.Out.WriteLine("No active connection was found");
                return;
            }

            IStatusIndicator status = ctx.AuthClient.Authenticate();

            if (status.Succeeded == false)
                _console.Out.WriteLine("Error: Authentication not successful: {0}", status.ErrorText);
        }
    }
}
