using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Iaik.Tc.TPM.Commands;

namespace Iaik.Tc.TPM
{
    public class TPMConsole
    {
        /// <summary>
        /// Values that can be used and set by the commands
        /// </summary>
        private Dictionary<string, object> _values = new Dictionary<string, object>();

        /// <summary>
        /// Tells the TPMConsole to read another command from the user
        /// </summary>
        private bool _runConsoleLoop = true;

		/// <summary>
		///Tells the command handler to exit on error 
		/// </summary>
		private bool _exitOnError = false;
		
		/// <summary>
		///Tells the script execution environment to exit if the script file has completed 
		/// </summary>
		private bool _exitOnFinish = true;
		
        /// <summary>
        /// Contains all Commands
        /// </summary>
        private IDictionary<string, IConsoleCommand> _commands = new SortedDictionary<string, IConsoleCommand>();

		
		/// <summary>
		///In script execution mode, the top contains the current script execution path 
		/// </summary>
		private Stack<string> _currentScriptExecutionPath = new Stack<string>();
		
        /// <summary>
        /// Returns a text writer where output of the commands is written to
        /// </summary>
        public TextWriter Out
        {
            get { return Console.Out; }
        }

        public TextReader In
        {
            get { return Console.In; }
        }

        public IEnumerable<KeyValuePair<string, IConsoleCommand>> Commands
        {
            get { return _commands; }
        }

        public bool RunConsoleLoop
        {
            get { return _runConsoleLoop; }
            set { _runConsoleLoop = value; }
        }

        /// <summary>
        /// Sets/overwrites the value with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetValue(string name, object value)
        {
            if (_values.ContainsKey(name))
                _values[name] = value;
            else
                _values.Add(name, value);
        }

        /// <summary>
        /// Gets the value with the specified name, converted to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetValue<T>(string name, T defaultValue)
        {
            if (_values.ContainsKey(name) == false)
                return defaultValue;
            else
            {
                object savedValue = _values[name];

                if (typeof(T).IsAssignableFrom(savedValue.GetType()))
                    return (T)savedValue;
                else
                    throw new InvalidCastException(string.Format("Cannot cast from '{0}' to '{1}'", savedValue.GetType(), typeof(T)));
            }
        }


        public TPMConsole()
        {
            //Looks for all ConsoleCommands
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsAbstract == false &&
                    t.IsClass &&
                    typeof(IConsoleCommand).IsAssignableFrom(t))
                {

                    object[] commandAttrs = t.GetCustomAttributes(typeof(TPMConsoleCommandAttribute), false);

                    if (commandAttrs != null && commandAttrs.Length == 1)
                    {
                        TPMConsoleCommandAttribute attribute = commandAttrs[0] as TPMConsoleCommandAttribute;
                        ConstructorInfo ctor = t.GetConstructor(new Type[] { });

                        if (ctor != null)
                        {
                            foreach (string cmdName in attribute.CmdNames)
                            {
                                if (_commands.ContainsKey(cmdName) == false)
                                    _commands.Add(cmdName, (IConsoleCommand)ctor.Invoke(new object[] { }));
                            }
                        }
                    }

                    foreach (IConsoleCommand cmd in _commands.Values)
                        cmd.Initialize(this);

                    object[] startupAttrs = t.GetCustomAttributes(typeof(TPMConsoleStartupCommand), false);

                    if (startupAttrs != null && startupAttrs.Length == 1)
                    {
                        ConstructorInfo ctor = t.GetConstructor(new Type[] { });

                        if (ctor != null)
                        {
                            IConsoleCommand cmd = (IConsoleCommand)ctor.Invoke(new object[] { });
                            cmd.Initialize(this);
                            cmd.Execute(null);
                        }
                    }
                }
            }


        }

        /// <summary>
        /// Runs the Console loop, returns on console exit
        /// </summary>
        public void Run ()
        {
        	while (_runConsoleLoop)
            {
        		
                Out.Write (":> ");
        		string commandLine = In.ReadLine ().Trim ();

                InterpretCommand (commandLine, false);
        	
            }
        }
		
		public void RunScriptFile (string scriptFile)
		{
			InnerRunScriptFile (scriptFile);
			
			if (_exitOnFinish == false)
			{
				Run ();
			}
		}
		
		private void InnerRunScriptFile (string scriptFile)
		{
			string realScriptFile;
			if (_currentScriptExecutionPath.Count () == 0)
			{
				//The first script that gets executed
				FileInfo scriptFileInfo = new FileInfo (scriptFile);
				_currentScriptExecutionPath.Push (scriptFileInfo.Directory.FullName);
				realScriptFile = scriptFile;
			}
			else
			{
				if (scriptFile.StartsWith ("."))
				{
					//Relative path
					realScriptFile = Path.Combine (_currentScriptExecutionPath.Peek (), scriptFile);
					_currentScriptExecutionPath.Push (new FileInfo (realScriptFile).Directory.FullName);
				}
				else
				{
					//absolute path
					realScriptFile = scriptFile;
					_currentScriptExecutionPath.Push (new FileInfo (scriptFile).Directory.FullName);
				}
			}
			
			
			using (StreamReader rdr = new StreamReader (File.OpenRead (realScriptFile))) 
			{
				string currentLine;
				while (_runConsoleLoop && rdr.EndOfStream == false)
				{
					currentLine = rdr.ReadLine ().Trim ();
					
					Console.WriteLine ("s > {0}", currentLine);
					
					if (currentLine.StartsWith ("#"))
					{
						continue;
					} 
					else if (currentLine.StartsWith ("@"))
					{
						InterpretSpecialCommands (currentLine);
					}	
					else
					{
						try
						{
							InterpretCommand (currentLine, true);
						}
						catch (Exception)
						{
							//Exception message output is already done in InterpretCommand
							if (_exitOnError)
								Environment.Exit (-1);
						}
					}
				}
			}
		}
		
		private void InterpretCommand (string commandLine, bool throwOnException)
		{
			if (!commandLine.Equals (String.Empty))
            {

                string[] commandParts = commandLine.Split (' ');
				if (commandParts.Length > 0 && _commands.ContainsKey (commandParts[0]))
                {
					try
                    {
						_commands[commandParts[0]].Execute (commandParts);
					}
                    catch (Exception ex)
                    {
						Out.WriteLine ("Error while executing command '{0}': {1}", commandParts[0], ex.Message);
						if (throwOnException)
							throw;
    				}
    			}
                else
    				Out.WriteLine ("Unknown command...");

            }
        	
		}
		
		
		private void InterpretSpecialCommands (string command)
		{
			string[] splittedCommand = command.Split (' ');
			
			if (splittedCommand[0].Equals ("@exit_on_error"))
			{
				_exitOnError = CmdLineBoolToBool (splittedCommand[1]);
			}
			else if (splittedCommand[0].Equals ("@exit_on_finish"))
			{
				_exitOnFinish = CmdLineBoolToBool (splittedCommand[1]);
			}
			else if (splittedCommand[0].Equals ("@include"))
			{
				InnerRunScriptFile (splittedCommand[1]);
			}
		}
		
		private bool CmdLineBoolToBool (string value)
		{
			if (value.Equals ("1"))
				return true;
			else if (value.Equals ("0"))
				return false;
			else
				throw new ArgumentException (string.Format("Could not parse '{0}' to bool", value)); 
		}

        
    }
}
