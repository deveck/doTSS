using System;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.Tpm.Context;
using System.Threading;

namespace Iaik.Tc.Tpm
{
	class MainClass
	{
		static volatile int _runningThreads = 0;
		
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
			
			object syncLock = new object();
			Random r = new Random();
			for(int i = 0; i< 10; i++)
			{
				Thread t = new Thread(new ThreadStart(delegate{
					
				try
					{
						
						_runningThreads ++;
					
						int myi = i;
				long serverTicks = ctx.DebugClient.PrintOnServerConsoleWithResponse("Hello with response");
						_runningThreads --;
				//Console.WriteLine("{1}Server ticks at execution: {0}", serverTicks, myi);
					}
					catch(Exception ex)
					{
						Console.WriteLine(ex);
					}
				}));
				
				t.Start();
			}
			
			
			//Thread tSupervisor = new Thread(new ThreadStart(delegate{
			while(_runningThreads > 0)
			{		
				lock(syncLock)
					Console.WriteLine("#{0} still running", _runningThreads);
					
				Thread.Sleep(10);
			}
			
			Console.WriteLine("#{0} still running", _runningThreads);
			
			//tSupervisor.Start();
			
			
			//Thread.Sleep(Timeout.Infinite);
		}
	}
}

