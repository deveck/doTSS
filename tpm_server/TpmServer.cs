///
///
/// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
/// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.ServiceProcess;

namespace iaik.tc.tpm
{	

	/// <summary>
	/// Entrypoint for the tpm server service and console application
	/// </summary>
	public class TpmServer : ServiceBase
	{
		/// <summary>
		/// Console entrypoint
		/// </summary>
		/// <param name="args">Command line arguments. you can override the default config file by supplying "--config=/path/to/configfile.conf"</param>
		public static void Main(string[] args)
		{
			
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