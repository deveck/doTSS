using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.Tpm.Context;

namespace Iaik.Tc.Tpm.Commands
{
    [TPMConsoleCommand("connection_info")]
    public class ConnectionInfoCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get { return @"connection_info Shows informations about the currently active connection"; }
        }

        public override void Execute(string[] commandline)
        {
            ClientContext ctx = _console.GetValue<ClientContext>("client_context", null);

            if (ctx == null)
                _console.Out.WriteLine("No active connection found");
            else
            {
                _console.Out.WriteLine("Active connection type: {0}", ctx.Connection.GetType());
                _console.Out.WriteLine("Connection state: {0}", ctx.Connection.Connected ? "Connected" : "Disconnected");
            }

        }
    }
}
