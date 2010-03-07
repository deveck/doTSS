using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Utils.CommonFactories;
using Iaik.Connection.ClientConnections;
using System.Reflection;

namespace Iaik.Tc.Tpm.Commands
{
    [TPMConsoleCommand("connection_create_info")]
    public class ConnectionCreateInfoCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"connection_create_info Args: [connection_name]
    Retrieves informations about the specified connection type";
            }
        }

        public override void Execute(string[] commandline)
        {
            if (commandline.Length < 2)
            {
                _console.Out.WriteLine("Error: [connection_name] not specified\n");
                return;
            }

            Type t = GenericClassIdentifierFactory.FindTypeForIdentifier<FrontEndConnection>(commandline[1]);

            if (t == null)
            {
                _console.Out.WriteLine("Error: Could not find connection with identifier '{0}'", commandline[1]);
                return;
            }

            _console.Out.WriteLine("'{0}' supports the following constructions:", commandline[1]);

            ConstructorInfo[] ctors = t.GetConstructors();
            //Currently the tpm console only supports string parameters, so look if there is a ctor who only nneds string arguments
            foreach (ConstructorInfo ctor in ctors)
            {
                bool validCtor = true;
                string ctorArguments = "";
                foreach (ParameterInfo param in ctor.GetParameters())
                {
                    ctorArguments += "[" + param.Name + "] ";
                    if (param.ParameterType.Equals(typeof(string)) == false)
                    {
                        validCtor = false;
                        break;
                    }
                }

                if (validCtor)
                {
                    _console.Out.WriteLine();
                    _console.Out.WriteLine(ctorArguments);
                }
            }

        }
    }
}
