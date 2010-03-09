// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Reflection;
using System.Collections.Generic;
using Iaik.Tc.Tpm.library.common;

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
			return Attribute.IsDefined(m, typeof(TpmCommandsAttribute)) &&
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
						TpmCommandsAttribute pattr = (TpmCommandsAttribute)Attribute.GetCustomAttribute(command, typeof(TpmCommandsAttribute));
						if(!commands_.Keys.Contains(pattr.CommandName))
							commands_.Add(pattr.CommandName, command);
					}
				}
			}
		}
		
		public static TpmCommand Create(TPMCommandRequest request)
		{
			return null;
			//
		}
	}
	
}