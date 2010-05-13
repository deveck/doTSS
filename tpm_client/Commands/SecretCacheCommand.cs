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

namespace Iaik.Tc.TPM.Commands
{
    [TPMConsoleCommand("secret_cache")]
    public class SecretCacheCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"secret_cache Args: [local_session_alias] [command] {args}
    Specify the tpm to use by [local_session_alias]. These aliases can be defined using the tpm_select command.
    All arguments supplied in {args} are comma seperated
    Available commands:
        clear            Clears all secrets recorded for the specified session
        
        add              args: type=owner[,secret=<secret>]
                               Adds or changes the cached owner secret
                               
                               type=srk[,secret=<secret>]
                               Adds or changes the cached SRK secret
                               
                               type=key_usage,name=<friendly name>[,secret=<secret>]
                               Adds or changes the usage secret for the key with the supplied friendly name
                               
                         If no secret argument is supplied, the secret is requested from the user (recommended method)
						
						 It is not neccessary to add secrets to the cache before they are used by the framework, if
						 a secret is not available it is automatically requested from the user, is is only needed to
						 add the secrets when using script files.
						
        remove           args: type=<secret type>
        
                         Removes the specified secret for the specified session from the secret cache						 
        
                            
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
        	string keyCommand = commandline[2];
   
			IDictionary<string, TPMSession> tpmSessions = _console.GetValue<IDictionary<string, TPMSession>> ("tpm_sessions", null);
   
			if (tpmSessions == null || tpmSessions.ContainsKey (localAlias) == false)
			{
        		_console.Out.WriteLine ("Error: Specified local alias was not found");
        		return;
        	}    
			
			
			if (keyCommand == "clear")
			{
        		List<string> toRemove = new List<string>();
        		
        		foreach(string key in tpmSessions[localAlias].ListValueKeys())
        		{
        			if(key.StartsWith("secret_"))
        				toRemove.Add(key);
        		}
        		
        		foreach(string key in toRemove)
        		{
        			tpmSessions[localAlias].ClearValue(key);
        		}
    			
        	}
        	else if (keyCommand == "remove")
			{
				
				IDictionary<string, string> arguments = null;
				
				if(commandline.Length >= 4)
					arguments = _console.SplitArguments(commandline[3], 0);
				
				if(commandline.Length < 4 || arguments.ContainsKey("type") == false)
				{
					_console.Out.WriteLine("Error: No type to remove specified");
					return;
				}
				
				tpmSessions[localAlias].ClearValue("secret_" + arguments["type"]);
				
			}
			else if(keyCommand == "add")
			{
				if(commandline.Length < 4)
				{
					_console.Out.WriteLine("Error: No arguments specified");
					return;
				}
			
				IDictionary<string, string> arguments = _console.SplitArguments(commandline[3], 0);
				
				if(arguments.ContainsKey("type") == false)
				{
					_console.Out.WriteLine("Error: No type specified");
					return;
				}
				
				string dictKey = arguments["type"];
				HMACKeyInfo keyInfo;
				Parameters hmacKeyInfoParams = new Parameters();
				if(dictKey == "owner")
				{
					dictKey = TPMSession.PARAM_AUTH_OWNER;
					keyInfo = new HMACKeyInfo(HMACKeyInfo.HMACKeyType.OwnerSecret, hmacKeyInfoParams);
				}
				else if(dictKey == "srk")
				{
					dictKey = TPMSession.PARAM_AUTH_SRK;
					keyInfo = new HMACKeyInfo(HMACKeyInfo.HMACKeyType.SrkSecret, hmacKeyInfoParams);
				}
				else if(dictKey == "key_usage")
				{
					if(arguments.ContainsKey("name") == false)
					{
						_console.Out.WriteLine("Error: key_usage requires name of key");
						return;
					}
					
					dictKey = "usage_" + arguments["name"];
					hmacKeyInfoParams.AddPrimitiveType("identifier", arguments["name"]);
					keyInfo = new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyUsageSecret, hmacKeyInfoParams);
				}
				else
				{
					_console.Out.WriteLine("Error: Unknown secret type");
					return;
				}
					
				
				ProtectedPasswordStorage pw;
				
				if(arguments.ContainsKey("secret"))
				{
					pw = new ProtectedPasswordStorage();
					foreach(char c in arguments["secret"])
						pw.AppendPasswordChar(c);
					
				}
				else
				{
					tpmSessions[localAlias].ClearValue("secret_" + dictKey);
					pw = tpmSessions[localAlias].RequestSecret(keyInfo);
				}	
				
				pw.Hash();
				tpmSessions[localAlias].SetValue("secret_" + dictKey, pw);
			}
			else
        		_console.Out.WriteLine ("Error, unknown command '{0}'", commandline[2]);
        }
    }
}
