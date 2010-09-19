//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.ServiceProcess;
using Iaik.Utils;
using System.Reflection;
using Iaik.Connection.ServerListeners;
using System.Threading;
using log4net;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.TPM.Context;
using System.Configuration;
using Iaik.Tc.TPM.Configuration;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration;
using System.Collections.Generic;
using Iaik.Utils.CommonFactories;
using Iaik.Connection.Configuration;
using Iaik.Utils.Locking;

namespace Iaik.Tc.TPM
{	
	
	/// <summary>
	/// Entrypoint for the tpm server service and console application
	/// </summary>
	public class TPMServer
	{
        /// <summary>
        /// Shows the commandline help
        /// </summary>
        /// <param name="command"></param>
        public static void OutputHelp (CommandLineHandler.CommandOption command)
        {
        	
			
            //Strange default .net versioning really makes sense ;-)
            DateTime buildDate = new DateTime(2000,1,1);
            buildDate = buildDate.AddDays(Assembly.GetEntryAssembly().GetName().Version.Build);
            buildDate = buildDate.AddSeconds(Assembly.GetEntryAssembly().GetName().Version.Revision * 2);

            Console.WriteLine("\n\n");
            Console.WriteLine("TPM Server v. {0}", Assembly.GetEntryAssembly().GetName().Version);
            Console.WriteLine("\tBuild on: {0} {1}", buildDate.ToShortDateString(), buildDate.ToLongTimeString());
            Console.WriteLine("\n\nSupported command line parameters:");
            Console.WriteLine("-help | -h              Show this fancy help");
            Console.WriteLine("-install-service | -i   Invoke Windows/Mono service installer");
            Console.WriteLine("-uninstall-service | -u Invoke Windows/Mono service uninstaller"); 
			
			Environment.Exit(0);
        }

		/// <summary>
		/// Console entrypoint
		/// </summary>
		/// <param name="args">Command line arguments. you can override the default config file by supplying "--config=/path/to/configfile.conf"</param>
		public static void Main (string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += HandleAppDomainCurrentDomainUnhandledException;
			
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.RegisterCallback("help", OutputHelp);
            commandLineHandler.RegisterCallback("h", OutputHelp);
            commandLineHandler.Parse(args);
			
	
			TPMServerContext ctx = new TPMServerContext();
			ctx.Start();
			
			Thread.Sleep(Timeout.Infinite);
		}

		static void HandleAppDomainCurrentDomainUnhandledException (object sender, UnhandledExceptionEventArgs e)
		{
			ILog log = LogManager.GetLogger("UNHANDLED_EXCEPTION");
			log.FatalFormat("{0}", e.ExceptionObject);
		}
	}
}
