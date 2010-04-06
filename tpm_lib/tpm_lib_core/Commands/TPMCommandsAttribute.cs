// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Library.Commands
{
    /// <summary>
    /// Attribute to flag a class as TPM command.    
    /// </summary> 
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class TPMCommandsAttribute : Attribute
	{
		private readonly String commandName_;
		
		public TPMCommandsAttribute(string commandName)
		{
			commandName_ = commandName;
		}
		
		public String CommandName
		{
			get
			{
				return commandName_;
			}				
		}
	}
}