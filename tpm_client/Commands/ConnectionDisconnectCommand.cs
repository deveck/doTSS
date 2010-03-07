using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Context;

namespace Iaik.Tc.Tpm.Commands
{
    [TPMConsoleCommand("connection_disconnect")]
    public class ConnectionDisconnectCommand:ConsoleCommandBase
    {
        public override string HelpText
        {
            get { return @"connection_disconnect Disconnects the currently active connection"; }
        }

        public override void Execute(string[] commandline)
        {
            ClientContext ctx = _console.GetValue<ClientContext>("client_context", null);

            if (ctx == null)
                _console.Out.WriteLine("No active connection was found");
            else
            {
                ctx.Connection.Dispose();
                _console.SetValue("client_context", null);
            }

        }
    }
}
