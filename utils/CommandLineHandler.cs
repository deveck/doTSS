// ///
// ///
// /// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// /// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections;
using System.Collections.Generic;

namespace iaik.utils
{
	
	/// <summary>
	/// Parses command line options by name and calls the appropriate (registered) handler 
	/// or the default handler which indicates that no command handler could be found for 
	/// the command line options
	/// </summary>
	/// <remarks>
	/// There are two types of command line switches, binary switches where the option is supplied or not, no further arguments re allowed for these switches.
	/// the second type are the default commandline switches in the form "-name=value" where everything behind the first '=' is taken as value 
	///</remarks>
	public class CommandLineHandler
	{
		/// <summary>
		/// Saves the CommandOption callback for each command
		/// </summary>
		private IDictionary<string, Action<CommandOption>> _optionCallbacks = new Dictionary<string, Action<CommandOption>>();
		
		/// <summary>
		/// Registers a new callback for the given commandoption name.
		/// If there is already an registered callback for the name provided, it gets overriden
		/// </summary>
		/// <param name="name"></param>
		/// <param name="callback"></param>
		public void RegisterCallback(string name, Action<CommandOption> callback)
		{
			if(_optionCallbacks.ContainsKey(name))
				_optionCallbacks[name] = callback;
			else
				_optionCallbacks.Add(name, callback);
		}
		
		public CommandLineHandler()
		{
		}
		
		/// <summary>
		/// Represents a single CommandOption parsed from the CommanLineHandler
		/// </summary>
		public class CommandOption
		{
			/// <summary>
			/// Defines the different available Command Option types
			/// </summary>
			public enum CommandOptionType
			{
				/// <summary>
				/// Binary option type where no further arguments are allowed:
				/// "--enable-this"
				/// </summary>
				Binary,
				
				/// <summary>
				/// Value option type where an argument is supplied with the command line option:
				/// "--name=value"
				/// </summary>
				Value
			}
			
			private CommandOptionType _optionType;
			
			/// <value>
			/// Returns the CommandOptionType (<see>CommandOptionType</see>)
			/// </value>
			public CommandOptionType OptionType
			{
				get{ return _optionType;}
			}
			
			private string _name;
			
			/// <value>
			/// Returns the name of the argument
			/// </value>
			public string Name
			{
				get{ return _name;}
			}
			
			private List<string> _arguments = new List<string>();
			
			/// <value>
			/// Returns the arguments of this CommandOption or an empty error if no option are available
			/// </value>
			public string[] Arguments
			{
				get{ return _arguments.ToArray();}
			}
			
			internal CommandOption(CommandOptionType optionType, string name, params string[] arguments)
			{
				_optionType = OptionType;
				_name = name;
				_arguments.AddRange(arguments);
			}
		}
	}
}
