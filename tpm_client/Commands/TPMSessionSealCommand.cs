using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils;
using Iaik.Tc.TPM.Keystore;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common.PCRData;
using System.IO;
using Iaik.Utils.IO;
using Org.BouncyCastle.Crypto;

namespace Iaik.Tc.TPM.Commands
{
    [TPMConsoleCommand("tpm_session_seal")]
    public class TPMSessionSealCommand : ConsoleCommandBase
    {
    	public enum DataInputMode
    	{
    		Embedded,
    		Console,
    		File
    	}
    	
    	public enum DataOutputMode
    	{
    		Console,
    		File
    	}
    	
    	public enum DataFormat
    	{
    		Raw,
    		Hex
    	}
    
        public override string HelpText
        {
            get
            {
                return @"tpm_session_seal Args: [local_session_alias]  {args} [embedded data]
    Specify the tpm to use by [local_session_alias]. These aliases can be defined using the tpm_select command.
    Available arguments: Arguments are seperated by ',' with NO spaces
                               
        name=<key_name>                Specifies the friendly name of the storage key to use
        
        pcr={1|2|5|...}                Specifies the PCRs the sealed data should be bind to
                               
        data_input={embedded,console,file}  Specifies the data input mode for the seal operation
          If data_input is embedded all data following {args} is interpreted as embedded data
          
        data_output={console,file}     Specifies the data output mode for the seal operation
          If data_output is file the output_file argument is required
        
        [input_data_format={>raw<,hex}]     Specifies the data format, raw is default.
                                       raw interprets data input from console and embedded as utf8 strings, from file the input is read bytewise
                                       hex input must be supplied in the following format 11f5c84c... alway 2 digits per byte          
                                       
        [output_data_format={>raw<,hex}]
                                       
        [file=filename]               Mandatory argument if using data_input=file. Specifies the file to read data to be sealed from
                            
        [output_file=filename]        Mandatory argument if using data_output= file.             
               
    	";    	
    
    
            }
        }

        public override void Execute (string[] commandline)
        {
        	if (commandline.Length < 2)
        	{
        		_console.Out.WriteLine ("Error: [local_session_alias] not specified");
        		return;
        	}
        	else if (commandline.Length < 3)
        	{
        		_console.Out.WriteLine ("Error: [command] not specified");
        		return;
        	}

            ClientContext ctx = _console.GetValue<ClientContext> ("client_context", null);

            if (ctx == null)
            {
        		_console.Out.WriteLine ("No active connection was found");
        		return;
        	}

			string localAlias = commandline[1];
   
			IDictionary<string, TPMSession> tpmSessions = _console.GetValue<IDictionary<string, TPMSession>> ("tpm_sessions", null);
   
			if (tpmSessions == null || tpmSessions.ContainsKey (localAlias) == false)
			{
        		_console.Out.WriteLine ("Error: Specified local alias was not found");
        		return;
        	}    
        	
        	if(tpmSessions[localAlias].Keystore == null)
			{
				_console.Out.WriteLine("Error: No keystore was opened");
				return;
			}
			
			string[] sArguments = commandline[2].Split(',');
    		IDictionary<string, string> arguments = _console.SplitArguments(commandline[2], 0);    
			
			if(arguments.ContainsKey("name") == false)
			{
				_console.Out.WriteLine("Error: no key name was specified");
				return;
			}
			
			if(arguments.ContainsKey("pcr") == false)
			{
				_console.Out.WriteLine("Error: no pcr values where specified");
				return;
			}
			
			if(arguments.ContainsKey("data_input") == false)
			{
				_console.Out.WriteLine("Error: no data input source specified");
				return;
			}
			
			DataInputMode dataInputMode;
			
			try
			{
				dataInputMode = (DataInputMode)Enum.Parse(typeof(DataInputMode), arguments["data_input"], true);
			}
			catch(Exception)
			{
				_console.Out.WriteLine("Error: Invalid data input source");
				return;
			}
			
			DataOutputMode dataOutputMode;
			
			try
			{
				dataOutputMode = (DataOutputMode)Enum.Parse(typeof(DataOutputMode), arguments["data_output"], true);
			}
			catch(Exception)
			{
				_console.Out.WriteLine("Error: Invalid data output destination");
				return;
			}
			
			DataFormat inputDataFormat = DataFormat.Raw;
			
			if(arguments.ContainsKey("input_data_format"))
			{
				try
				{
					inputDataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), arguments["input_data_format"], true);
				}
				catch(Exception)
				{
					_console.Out.WriteLine("Error: Invalid input data format");
					return;
				}
			}
			
			DataFormat outputDataFormat = DataFormat.Raw;
			
			if(arguments.ContainsKey("output_data_format"))
			{
				try
				{
					outputDataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), arguments["output_data_format"], true);
				}
				catch(Exception)
				{
					_console.Out.WriteLine("Error: Invalid output data format");
					return;
				}
			}
			
			string file = null;
			
			if(dataInputMode == DataInputMode.File && arguments.ContainsKey("file") == false)
			{
				_console.Out.WriteLine("Error: data_input=file requires file argument!");
				return;
			}
			
			string outputFile = null;
			
			if(dataOutputMode == DataOutputMode.File && arguments.ContainsKey("output_file") == false)
			{
				_console.Out.WriteLine("Error: data_output=file requires output_file argument!");
				return;
			}
			
			TPMPCRSelection pcrSelection = tpmSessions[localAlias].CreateEmptyPCRSelection();
			
			foreach(string pcr in arguments["pcr"].Split('|'))
			{
				int pcrValue = int.Parse(pcr);				
				
				pcrSelection.PcrSelection.SetBit(pcrValue - 1, true);				
			}
			
			ClientKeyHandle keyHandle = tpmSessions[localAlias].KeyClient.GetKeyHandleByFriendlyName(arguments["name"]);
			
			
			Stream inputStream = null;
			
			if(dataInputMode == DataInputMode.Console)
				inputStream = new TextReaderStream(_console.In);
			else if(dataInputMode == DataInputMode.Embedded)
			{
				if(commandline.Length <= 3)
				{
					_console.Out.WriteLine("Error: no embedded data");
					return;
				}
				
				StringBuilder embeddedData = new StringBuilder();
				for(int i = 3; i<commandline.Length; i++)
				{
					embeddedData.Append(commandline[i]);
					if(i + 1 < commandline.Length)
						embeddedData.Append(" ");
				}
				
				inputStream = new TextReaderStream(new StringReader(embeddedData.ToString()));
			}
			else if(dataInputMode == DataInputMode.File)
			{
				inputStream = new FileStream(arguments["file"], FileMode.Open, FileAccess.Read);
			}
			
			if(inputDataFormat == DataFormat.Hex)
			{
				inputStream = new HexFilterStream(inputStream);
			}
			
			Stream outputStream = null;
			
			if(dataOutputMode == DataOutputMode.Console)
				outputStream = new HexFilterStream(new TextWriterStream(_console.Out));
			else if(dataOutputMode == DataOutputMode.File)
			{
				if(outputDataFormat == DataFormat.Hex)
					outputStream = new HexFilterStream(new FileStream(arguments["output_file"], FileMode.OpenOrCreate | FileMode.Truncate, FileAccess.Write));
				else
					outputStream = new FileStream(arguments["output_file"], FileMode.OpenOrCreate, FileAccess.Write);
			}
			
			
			IAsymmetricBlockCipher sealCipher = keyHandle.CreateSealBlockCipher(pcrSelection);
			sealCipher.Init(true, null);
			
			int read;
			byte[] buffer = new byte[sealCipher.GetInputBlockSize()];
			do
			{
				read = inputStream.Read(buffer, 0, buffer.Length);
				
				if(read > 0)
				{
					byte[] encrypted = sealCipher.ProcessBlock(buffer, 0, read);
					outputStream.Write(encrypted, 0, encrypted.Length);
				}
			}while(read>0);

			_console.Out.WriteLine();
			outputStream.Dispose();
			inputStream.Dispose();
									
        }        
    }
}
