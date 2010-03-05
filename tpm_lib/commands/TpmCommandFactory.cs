// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Reflection;
using System.Collections.Generic;

namespace Iaik.Tc.Tpm.library.commands
{
	public static class TpmCommandFactory
	{
		private static readonly IDictionary<String, Type> commands_ = new Dictionary<String, Type>();
		
		static TpmCommandFactory()
		{
			RegisterCommands(typeof(TpmCommandFactory).Assembly);
		}
		
		private static bool TpmCommandFilter(Type m, Object criteria)
		{
			return Attribute.IsDefined(m, typeof(TpmCommandAttribute)) &&
				!m.IsAbstract &&
					typeof(TpmCommand).IsAssignableFrom(m);
		}
		
		public static void RegisterCommands(Assembly assembly)
		{
			lock(commands_)
			{
				foreach(Module module in assembly.GetModules())
				{
					foreach(Type command in module.FindTypes(TpmCommandFilter, null))
					{
						
					}
				}
			}
		}
		
		public static TpmCommand Create(TPMCommandRequest request)
		{
			//create();
		}
	}
	
}