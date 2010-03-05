// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.Tpm.library.commands
{
    /// <summary>
    /// Attribute to flag a class as TPM command.    
    /// </summary> 
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class TpmCommandsAttribute : Attribute
	{
		private readonly String commandName_;
		
		public TpmCommandsAttribute(string commandName)
		{
			commandName_ = commandName;
		}
		
		public String commandName
		{
			get
			{
				return commandName_;
			}				
		}
	}
}