using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Utils.CommonFactories;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.TPM.Context;
using Iaik.Utils.Hash;

namespace Iaik.Tc.TPM.Commands
{
    [TPMConsoleCommand("connection_create")]
    public class ConnectionCreateCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get 
            {
                return @"connection_create  Args: [connection_name] [connection_arguments]
    Creates a new connection of the specified type. The arguments of 
    a specific connection type can be retrieved using 
    connection_create_info";
            }
        }

        public override void Execute(string[] commandline)
        {
            if (commandline.Length < 2)
            {
                _console.Out.WriteLine("Error: [connection_name] not specified");
                return;
            }

            //string[] arguments = new string[commandline.Length - 2];
			
            // if(arguments.Length > 0)
            //Array.Copy(commandline, 2, arguments, 0, arguments.Length);

		    IDictionary<string, string> arguments = _console.SplitArguments(commandline[2], 0);
			
            //FrontEndConnection conn = GenericClassIdentifierFactory.CreateFromClassIdentifierOrType<FrontEndConnection>(commandline[1], arguments);
			ConnectionBuilderSettings settings = new ConnectionBuilderSettings(RequestSecret);
			FrontEndConnection conn = ConnectionFactory.CreateFrontEndConnection(commandline[1], settings, arguments);
			
            if (conn == null)
            {
                _console.Out.WriteLine("Error: could not create connection '{0}' identifier not found or cannot construct, run connection_create_info for more information");
                return;
            }
			
			
			conn.Connect();
			

            ClientContext ctx = EndpointContext.CreateClientEndpointContext(conn);
			ctx.TPMClient.SetRequestSecretCallback(_console.AsyncSecretRequestCallback);
            _console.SetValue("client_context", ctx);
        }
		
		private ProtectedPasswordStorage RequestSecret(string hintText)
		{
			SecretRequest request = new SecretRequest(hintText);
			_console.AddSecretRequest(request);
			request.PasswordReady.WaitOne();
			return request.ProtectedPassword;
		}
    }
	
	
}
