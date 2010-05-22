using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using Iaik.Tc.TPM.Commands;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using log4net;

namespace Iaik.Tc.TPM
{
    public class TPMConsole
    {
  		/// <summary>
		/// Logger
		/// </summary>
		protected ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	
		
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
		/// In script execution mode, the top contains the current script execution path 
		/// </summary>
		private Stack<string> _currentScriptExecutionPath = new Stack<string>();
		
		/// <summary>
		/// Contains all currently queued secret requests 
		/// </summary>
		private Queue<SecretRequest> _secretRequests = new Queue<SecretRequest>();
		
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
        
        /// <summary>
        /// Lists all added keys 
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerable<System.String>"/>
        /// </returns>
        public IEnumerable<string> ListValueKeys()
        {
        	return _values.Keys;
        }

		public void ClearValue(string name)
		{
			_values.Remove(name);
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

		private volatile bool _commandReady = true;
		
		/// <summary>
        /// Runs the Console loop, returns on console exit
        /// </summary>
        public void Run ()
        {
			bool outputPrompt = true;
			StringBuilder currentCommandLine = new StringBuilder();
			
			bool commandRunning = false;
			
        	while (_runConsoleLoop)
            {
        	
				if(commandRunning == false && outputPrompt)
				{
                	Out.Write (":> ");
					outputPrompt = false;
				}
				
				while(commandRunning == false && Console.KeyAvailable)
				{
					ConsoleKeyInfo keyInfo = Console.ReadKey();
					
					if(keyInfo.Key == ConsoleKey.Backspace)
					{
						if(currentCommandLine.Length > 0)
							currentCommandLine.Remove(currentCommandLine.Length - 1, 1);
					}
					else if(keyInfo.Key == ConsoleKey.Enter)
					{
						_commandReady = false;
						commandRunning = true;
						InterpretCommand(currentCommandLine.ToString(), false, false);
						
						currentCommandLine.Remove(0, currentCommandLine.Length);
						outputPrompt = true;
					}
					else
						currentCommandLine.Append(keyInfo.KeyChar);
				}
				
				if(_commandReady)
				{
					commandRunning = false;
				}
				
				SecretRequest secretRequest = null;
				lock(_secretRequests)
				{
					if(_secretRequests.Count > 0)
						secretRequest = _secretRequests.Dequeue();
				}
				
				if(secretRequest != null)
				{
					HandleSecretRequest(secretRequest);	
					currentCommandLine.Remove(0, currentCommandLine.Length);
				}
				
				Thread.Sleep(10);				
        	
            }
        }
		
		private void HandleSecretRequest(SecretRequest request)
		{
			if(request.KeyInfo.KeyType == HMACKeyInfo.HMACKeyType.OwnerSecret)
				request.ProtectedPassword = Utils.ReadPassword("Server requests owner password:", this, false);
			else if(request.KeyInfo.KeyType == HMACKeyInfo.HMACKeyType.SrkSecret)
				request.ProtectedPassword = Utils.ReadPassword("Server requests srk password:", this, false);
			else if(request.KeyInfo.KeyType == HMACKeyInfo.HMACKeyType.KeyUsageSecret)
				request.ProtectedPassword = Utils.ReadPassword(string.Format("Server requests usage secret for key '{0}'",
					request.KeyInfo.Parameters.GetValueOf<string>("identifier")), this, false);
			else if(request.KeyInfo.KeyType == HMACKeyInfo.HMACKeyType.SealAuth)
				request.ProtectedPassword = Utils.ReadPassword(string.Format("Server requests auth for pending seal operation"), this, false);
			else
				throw new ArgumentException("Key type not supported by TPM console");
			
			request.PasswordReady.Set();
			
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
							InterpretCommand (currentLine, true, true);
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
		
		private void  InterpretCommand (string commandLine, bool throwOnException, bool sync)
		{
			AutoResetEvent evt = null;
			
			if(sync)
				evt = new AutoResetEvent(false);
			
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				try
				{
					//if (!commandLine.Equals (String.Empty))
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
								Out.WriteLine ("Error while executing command '{0}': {1}", commandParts[0], ex);
								if (throwOnException)
									throw;
		    				}
							finally
							{
								_commandReady = true;
							}
		    			}
		                else
						{
		    				Out.WriteLine ("Unknown command...");
							_commandReady = true;
						}
						
		
		            }
				}
				finally
				{
					if(sync)
						evt.Set();
				}
				});
				
			if(sync)
				evt.WaitOne();
			
			
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

		
		public ConsoleKeyInfo ReadKeyWithoutEcho ()
		{
			return Console.ReadKey (true);
		}
		
		public void AddSecretRequest(SecretRequest secretRequest)		
		{
			lock(_secretRequests)
				_secretRequests.Enqueue(secretRequest);
		}
		
		public ProtectedPasswordStorage AsyncSecretRequestCallback(HMACKeyInfo keyInfo)
		{
			_logger.DebugFormat("Async requesting secret '{0}'", keyInfo.KeyType);			
			
			SecretRequest request = new SecretRequest(keyInfo);
			AddSecretRequest(request);
			request.PasswordReady.WaitOne();
			return request.ProtectedPassword;
		}
		
		public IDictionary<string, string> SplitArguments(string sArguments, int startIndex)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();
			
			string[] splittedArguments = sArguments.Split(',');
			if(splittedArguments.Length > startIndex)
			{
				for(int i = startIndex; i<splittedArguments.Length; i++)
				{
					string[] splittedArgument = splittedArguments[i].Split('=');
					arguments.Add(splittedArgument[0], splittedArgument[1]);
				}
			}
			
			return arguments;
		}
			
	}
}