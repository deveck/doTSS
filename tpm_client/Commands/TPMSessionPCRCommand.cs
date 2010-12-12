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
using System.IO;
using Iaik.Utils.Hash;
using Iaik.Utils.IO;
using Iaik.Tc.TPM.Library.Common.PCRData;

namespace Iaik.Tc.TPM.Commands
{
    [TPMConsoleCommand("tpm_session_pcr")]
    public class TPMSessionPCRCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"tpm_session_pcr Args: [local_alias] [pcr_subcommand]
    Specify the tpm to use by [local_alias]. These aliases can be defined using the tpm_select command.
	
        pcr_report    Reports the status of all platform configuration registers

        extend        Extends the specified pcr by the specified data or digest
           Arguments:
             pcr=[pcrnum] Specifies the pcr to be extended

             data_input={embedded,file}  Specifies the data input mode for the seal operation

                embedded:
                   The last argument is interpreted as digest in hex format 
                   without spaces. Keep in mind that the digest needs to be
                   exactly 20 bytes
    
                file:
                   The file specified by the 'file' parameter is hashed using
                   SHA1, this is used as digest.
          
                                       
             [file=filename]  Mandatory argument if using data_input=file. 
                              Specifies the file to read data to be hashed from
                              and extended
                              
         quote         Cryptographic reporting of PCR values
         	Arguments:
             pcr=[pcr1|pcr2|...|pcrn] Specifies the pcrs to be quoted
		";
	
            }
        }

        public override void Execute (string[] commandline)
        {
        	if (commandline.Length < 2)
        		_console.Out.WriteLine ("Error: [local_alias] not specified");
        	else if (commandline.Length < 3)
        		_console.Out.WriteLine ("Error: [pcr_subcommand] not specified");

            ClientContext ctx = _console.GetValue<ClientContext> ("client_context", null);

            if (ctx == null)
            {
        		_console.Out.WriteLine ("No active connection was found");
        		return;
        	}

			string localAlias = commandline[1];
        	string pcrCommand = commandline[2];
   
			IDictionary<string, TPMSession> tpmSessions = _console.GetValue<IDictionary<string, TPMSession>> ("tpm_sessions", null);
   
			if (tpmSessions == null || tpmSessions.ContainsKey (localAlias) == false)
			{
        		_console.Out.WriteLine ("Error: Specified local alias was not found");
        		return;
        	}
    
			
			
			if (pcrCommand == "report")
			{
				uint pcrCount = tpmSessions[localAlias].CapabilityClient.GetPCRCount();
				
				for(uint i = 0; i<pcrCount; i++)
					_console.Out.WriteLine("#{0}: {1}", i, ByteHelper.ByteArrayToHexString(tpmSessions[localAlias].IntegrityClient.PCRValue(i)));
			
        	}
			else if(pcrCommand == "extend")
			{
				if(commandline.Length < 4)
				{
					_console.Out.WriteLine("Error: 'extend' requires some arguments");
					return;
				}
				IDictionary<string, string> arguments =_console.SplitArguments(commandline[3], 0);
				
				if(arguments.ContainsKey("pcr") == false)
				{
					_console.Out.WriteLine("Error: 'extend' requires parameter 'pcr' to be specified");
					return;
				}
				
				uint pcr = 0;
				
				if(uint.TryParse(arguments["pcr"], out pcr) == false)
				{
					_console.Out.WriteLine("Error: 'pcr' could not be parsed, is it a valid pcr specified?");
					return;
				}
				
				if(arguments.ContainsKey("data_input") == false)
				{
					_console.Out.WriteLine("Error: 'extend' requires parameter 'data_input' to be specified");
					return;
				}
				
				TPMSessionSealCommand.DataInputMode dataInput = 
					(TPMSessionSealCommand.DataInputMode)Enum.Parse(typeof(TPMSessionSealCommand.DataInputMode), arguments["data_input"], true);

				if(dataInput != TPMSessionSealCommand.DataInputMode.Embedded &&
				   dataInput != TPMSessionSealCommand.DataInputMode.File)
				{
					_console.Out.WriteLine("Error: 'data_input' has an invalid value");
					return;
				}
				
				byte[] digest;
				
				if(dataInput == TPMSessionSealCommand.DataInputMode.File &&
				   arguments.ContainsKey("file") == false)
				{
					_console.Out.WriteLine("Error: file-data_input require 'file' argument to be specified");
					return;
				}
				else if(dataInput == TPMSessionSealCommand.DataInputMode.File)
				{
					FileInfo myFile = new FileInfo(arguments["file"]);
					using(FileStream src = myFile.OpenRead())
					{
						digest = new HashProvider().Hash(
						        new HashStreamDataProvider(src, null, null, false));
					}
				}
				else if(dataInput == TPMSessionSealCommand.DataInputMode.Embedded)
				{
					using(Stream src = new HexFilterStream(new TextReaderStream( new StringReader(commandline[4]))))
					{
						digest = new byte[20];
						if(src.Length != 20)
						{
							throw new ArgumentException("Error: The embedded digest must be 20 bytes long");
						}
						
						src.Read(digest, 0, 20);						
					}
				}
				else
					throw new ArgumentException(String.Format("data input mode '{0}' is not supported", dataInput));
				
				_console.Out.WriteLine("Doing extension with digest: '{0}'", ByteHelper.ByteArrayToHexString(digest));
				
				byte[] newDigest = tpmSessions[localAlias].IntegrityClient.Extend(pcr, digest);
				_console.Out.WriteLine("Extension successful, new pcr value:  {0}", ByteHelper.ByteArrayToHexString(newDigest));
			}
			else if(pcrCommand == "quote")
			{
				if(commandline.Length < 4)
				{
					_console.Out.WriteLine("Error: 'quote' requires some arguments");
					return;
				}
				
				IDictionary<string, string> arguments =_console.SplitArguments(commandline[3], 0);
				
				if(arguments.ContainsKey("pcr") == false)
				{
					_console.Out.WriteLine("Error: 'quote' requires parameter 'pcr' to be specified");
					return;
				}
				
				if(arguments.ContainsKey("name") == false)
				{
					_console.Out.WriteLine("Error: no key name was specified");
					return;
				}

                ClientKeyHandle keyHandle = tpmSessions[localAlias].KeyClient.GetKeyHandleByFriendlyName(arguments["name"]);
				
				TPMPCRSelection pcrSelection = tpmSessions[localAlias].CreateEmptyPCRSelection();
				
				foreach(string pcr in arguments["pcr"].Split('|'))
				{
					int pcrValue = int.Parse(pcr);				
					pcrSelection.PcrSelection.SetBit(pcrValue - 1, true);				
				}
				
				TPMPCRComposite quoted = keyHandle.SimpleQuote(pcrSelection);

                IList<int> selectedPCRs = quoted.PCRSelection.SelectedPCRs;

                for (int i = 0; i < selectedPCRs.Count; i++)
                {
                    _console.Out.WriteLine("#{0}: {1}", selectedPCRs[i], ByteHelper.ByteArrayToHexString(quoted.PCRValues[i]));
                }
				
			}
			else
        		_console.Out.WriteLine ("Error, unknown pcr_subcommand '{0}'", commandline[1]);
        }
    }
}
