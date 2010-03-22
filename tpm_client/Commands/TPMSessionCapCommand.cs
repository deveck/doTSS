using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Commands
{
    [TPMConsoleCommand("tpm_session_cap")]
    public class TPMSessionCapCommand : ConsoleCommandBase
    {
        public override string HelpText
        {
            get
            {
                return @"tpm_session_cap Args: [local_alias] [cap_type]
    Specify the tpm to use by [local_alias]. These aliases can be defined using the tpm_select command
    Returns the capability specified by cap_type by one of these values: tpm_version";
            }
        }

        public override void Execute (string[] commandline)
        {
        	if (commandline.Length < 2)
        		_console.Out.WriteLine ("Error: [local_alias] not specified");
        	else if (commandline.Length < 3)
        		_console.Out.WriteLine ("Error: [cap_type] not specified");

            ClientContext ctx = _console.GetValue<ClientContext> ("client_context", null);

            if (ctx == null)
            {
        		_console.Out.WriteLine ("No active connection was found");
        		return;
        	}

			string localAlias = commandline[1];
        	string capCommand = commandline[2];
   
			IDictionary<string, TPMSession> tpmSessions = _console.GetValue<IDictionary<string, TPMSession>> ("tpm_sessions", null);
   
			if (tpmSessions == null || tpmSessions.ContainsKey (localAlias) == false)
			{
        		_console.Out.WriteLine ("Error: Specified local alias was not found");
        		return;
        	}
    
			
			
			if (capCommand == "tpm_version")
			{
        		CapabilityData.TPMCapVersionInfo versionInfo = tpmSessions[localAlias].CapabilityClient.GetTPMVersion ();
    
				_console.Out.WriteLine ("major: {0}, minor: {1}, rev major: {2}, rev minor: {3}", versionInfo.Version.Major, 
					versionInfo.Version.Minor, versionInfo.Version.RevMajor, versionInfo.Version.RevMinor);
    
				_console.Out.WriteLine ("Speclevel: {0} errataRev: {1}", versionInfo.SpecLevel, versionInfo.ErrataRev);
        		_console.Out.WriteLine ("VendorId: {0}", ByteHelper.ByteArrayToHexString (versionInfo.TpmVendorId));
        		_console.Out.WriteLine ("Vendor specific (size #{0} bytes): {1}", versionInfo.VendorSpecific.Length,
					ByteHelper.ByteArrayToHexString (versionInfo.VendorSpecific));
        	}
			else
        		_console.Out.WriteLine ("Error, unknown cap_type '{0}'", commandline[1]);
        }
    }
}