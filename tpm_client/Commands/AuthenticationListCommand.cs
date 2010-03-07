using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Context;

namespace Iaik.Tc.Tpm.Commands
{
    [TPMConsoleCommand("auth_list")]
    public class AuthenticationListCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get { return "auth_list Lists all available and compatible authorisation methods"; }
        }

        public override void Execute(string[] commandline)
        {
            ClientContext ctx = _console.GetValue<ClientContext>("client_context", null);

            if (ctx == null)
            {
                _console.Out.WriteLine("No client connection was found!");
                return;
            }

            _console.Out.WriteLine("Supported authentication methods:");
            foreach (string authMethod in ctx.AuthClient.SupportedAuthenticationMethods)
            {
                _console.Out.WriteLine("\t{0}", authMethod);
            }
        }
    }
}
