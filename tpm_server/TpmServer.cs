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
using Iaik.Tc.Tpm.Context;
using System.Configuration;
using Iaik.Tc.Tpm.Configuration;
using Iaik.Tc.Tpm.Configuration.DotNetConfiguration.Elements;

namespace Iaik.Tc.Tpm
{	
	
	/// <summary>
	/// Entrypoint for the tpm server service and console application
	/// </summary>
	public class TpmServer : ServiceBase
	{
        /// <summary>
        /// Shows the commandline help
        /// </summary>
        /// <param name="command"></param>
        public static void OutputHelp(CommandLineHandler.CommandOption command)
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
        }

		/// <summary>
		/// Console entrypoint
		/// </summary>
		/// <param name="args">Command line arguments. you can override the default config file by supplying "--config=/path/to/configfile.conf"</param>
		public static void Main(string[] args)
		{
			AccessControlListSection sect = (AccessControlListSection)System.Configuration.ConfigurationManager.GetSection("accessControlList");
			foreach(string key in System.Configuration.ConfigurationManager.AppSettings.AllKeys)
				Console.WriteLine(key);
			
			Environment.Exit(0);
			
			log4net.Appender.ConsoleAppender appender = new log4net.Appender.ConsoleAppender();
			appender.Name = "ConsoleAppender";
			appender.Layout = new log4net.Layout.PatternLayout("[%date{dd.MM.yyyy HH:mm:ss,fff}]-%-5level-[%type]: %message%newline");
			log4net.Config.BasicConfigurator.Configure(appender);
			
			ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			
            CommandLineHandler commandLineHandler = new CommandLineHandler();
            commandLineHandler.RegisterCallback("help", OutputHelp);
            commandLineHandler.RegisterCallback("h", OutputHelp);
            commandLineHandler.Parse(args);
			
			//TODO: Read configuration and do not load the hard coded listeners ;)
			
			UnixSocketListener listener = new UnixSocketListener("/tmp/tpm_testsocket");
            //NamedPipeListener listener = new NamedPipeListener();
			listener.ClientConnected += HandleListenerClientConnected;
			listener.Listen();
			
			Thread.Sleep(Timeout.Infinite);
		}

		static void HandleListenerClientConnected (FrontEndConnection connection)
		{
			ServerContext ctx = EndpointContext.CreateServerEndpointContext(connection);
			
			//TODO: add generated context to some sort of list, to know which connections are open
		}
		
		protected override void OnStart (string[] args)
		{
			base.OnStart (args);
		}
		
		protected override void OnStop ()
		{
			base.OnStop ();
		}


	}
}