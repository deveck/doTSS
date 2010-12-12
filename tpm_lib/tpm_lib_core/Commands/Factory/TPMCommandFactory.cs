/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Reflection;
using System.Collections.Generic;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Library.Commands
{
	public static class TPMCommandFactory
	{
		private static readonly IDictionary<String, Type> commands_ = new Dictionary<String, Type>();
		
		static TPMCommandFactory()
		{
			RegisterCommands(typeof(TPMCommandFactory).Assembly);
		}
		
		private static bool TpmCommandFilter(Type m, Object criteria)
		{
			return Attribute.IsDefined(m, typeof(TPMCommandsAttribute)) &&
				!m.IsAbstract &&
					typeof(TPMCommand).IsAssignableFrom(m);
		}
		
		public static void RegisterCommands(Assembly assembly)
		{
			lock(commands_)
			{
				foreach(Module module in assembly.GetModules())
				{
					foreach(Type command in module.FindTypes(TpmCommandFilter, null))
					{
						TPMCommandsAttribute pattr = (TPMCommandsAttribute)Attribute.GetCustomAttribute(command, typeof(TPMCommandsAttribute));
						if(!commands_.Keys.Contains(pattr.CommandName))
							commands_.Add(pattr.CommandName, command);
					}
				}
			}
		}
		
		public static TPMCommand Create (TPMCommandRequest request)
		{
			if (commands_.ContainsKey (request.CommandIdentifier))
			{
				ConstructorInfo ctorInfo = commands_[request.CommandIdentifier].GetConstructor (new Type[0]);
				if (ctorInfo == null)
					throw new ArgumentException (string.Format ("Cannot create TpmCommand for command request with identifier '{0}'", 
						request.CommandIdentifier));
				
				return (TPMCommand)ctorInfo.Invoke (new object[0]);
			}
			else
				throw new NotSupportedException (string.Format ("Cannot find TpmCommand for command request with identifier '{0}'", 
						request.CommandIdentifier));
		}
		
		public static TPMCommand Create (string identifier)
		{
			if (commands_.ContainsKey (identifier))
			{
				ConstructorInfo ctorInfo = commands_[identifier].GetConstructor (new Type[0]);
				if (ctorInfo == null)
					throw new ArgumentException (string.Format ("Cannot create TpmCommand for command request with identifier '{0}'", 
						identifier));
				
				return (TPMCommand)ctorInfo.Invoke (new object[0]);
			}
			else
				throw new NotSupportedException (string.Format ("Cannot find TpmCommand for command request with identifier '{0}'", 
						identifier));
		}
	}
	
}