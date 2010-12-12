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
    [TPMConsoleCommand("tpm_session_signature")]
    public class TPMSessionSignCommand : ConsoleCommandBase
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
                return @"tpm_session_sign Args: [local_session_alias]  {args} [embedded data]
    Specify the tpm to use by [local_session_alias]. These aliases can be defined using the tpm_select command.
    Available arguments: Arguments are seperated by ',' with NO spaces
                               
        name=<key_name>                Specifies the friendly name of the storage key to use
        
        data_input={embedded,console,file}  Specifies the data input mode for the sign operation
          If data_input is embedded all data following {args} is interpreted as embedded data
                 
        [input_data_format={>raw<,hex}]     Specifies the data format, raw is default.
                                       raw interprets data input from console and embedded as utf8 strings, from file the input is read bytewise
                                       hex input must be supplied in the following format 11f5c84c... alway 2 digits per byte          
                                       
        [file=filename]               Mandatory argument if using data_input=file. Specifies the file to read data to be sealed from
                                           
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
			
			string subCommand = commandline[2];
			IDictionary<string, string> arguments = _console.SplitArguments(commandline[3], 0);    
			
			if(arguments.ContainsKey("name") == false)
			{
				_console.Out.WriteLine("Error: no key name was specified");
				return;
			}
			
//			if(arguments.ContainsKey("pcr") == false)
//			{
//				_console.Out.WriteLine("Error: no pcr values where specified");
//				return;
//			}
			
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
	
			
			if(dataInputMode == DataInputMode.File && arguments.ContainsKey("file") == false)
			{
				_console.Out.WriteLine("Error: data_input=file requires file argument!");
				return;
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
			
			

			ISigner signatureGenerator = null;
			
			
			if(subCommand == "verify")
			{
				signatureGenerator = keyHandle.CreateSigner();
				signatureGenerator.Init(false, null);
			}
			else if(subCommand == "generate")
			{
				signatureGenerator = keyHandle.CreateSigner();
				signatureGenerator.Init(true, null);
			}
			else if(subCommand == "generate_quote" || subCommand == "verify_quote")
			{
				if(arguments.ContainsKey("pcr") == false)
				{
					_console.Out.WriteLine("Error: No pcrs specified!");
					return;
				}
				
				TPMPCRSelection pcrSelection = tpmSessions[localAlias].CreateEmptyPCRSelection();
				
				foreach(string pcr in arguments["pcr"].Split('|'))
				{
					int pcrValue = int.Parse(pcr);				
					pcrSelection.PcrSelection.SetBit(pcrValue - 1, true);				
				}
				
				signatureGenerator = keyHandle.CreateQuoter(pcrSelection);
				signatureGenerator.Init(subCommand == "generate_quote", null);
			}
			
			byte[] buffer = new byte[1024];
			int read = 0;
			
			do
			{
				read = inputStream.Read(buffer, 0, buffer.Length);
				
				signatureGenerator.BlockUpdate(buffer, 0, read);
			}
			while(read > 0);
			
			_console.Out.WriteLine(ByteHelper.ByteArrayToHexString(signatureGenerator.GenerateSignature()));
			_console.Out.WriteLine();
			inputStream.Dispose();
									
        }        
    }
}
