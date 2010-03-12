using System;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.Tpm.Context;
using System.Threading;

namespace Iaik.Tc.Tpm
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            log4net.Appender.DebugAppender appender = new log4net.Appender.DebugAppender();
            appender.Name = "DebugAppender";
			appender.Layout = new log4net.Layout.PatternLayout("[%date{dd.MM.yyyy HH:mm:ss,fff}]-%-5level-[%type]: %message%newline");
			log4net.Config.BasicConfigurator.Configure(appender);


            new TPMConsole().Run();
		}
	}
}

