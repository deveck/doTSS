using System;
using Iaik.Tc.Tpm.Connection.ClientConnections;
using Iaik.Tc.Tpm.Context;
using System.Threading;

namespace Iaik.Tc.Tpm
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			log4net.Appender.ConsoleAppender appender = new log4net.Appender.ConsoleAppender();
			appender.Name = "ConsoleAppender";
			appender.Layout = new log4net.Layout.PatternLayout("[%date{dd.MM.yyyy HH:mm:ss,fff}]-%-5level-[%type]: %message%newline");
			log4net.Config.BasicConfigurator.Configure(appender);
			
			FrontEndConnection tpmConnection = new UnixSocketConnection("/tmp/tpm_testsocket");
            //FrontEndConnection tpmConnection = new NamedPipeConnection("TPM_csharp");
			tpmConnection.Connect();
			
			
			ClientContext ctx = EndpointContext.CreateClientEndpointContext(tpmConnection);
			ctx.DebugClient.PrintOnServerConsole("Hello from client");
			
			
			for(int i = 0; i< 100; i++)
			{
				Thread t = new Thread(new ThreadStart(delegate{
					
					int myi = i;
				long serverTicks = ctx.DebugClient.PrintOnServerConsoleWithResponse("Hello with response");
				Console.WriteLine("{1}Server ticks at execution: {0}", serverTicks, myi);
				}));
				
				t.Start();
			}
			
			
			
			
			Thread.Sleep(Timeout.Infinite);
		}
	}
}

