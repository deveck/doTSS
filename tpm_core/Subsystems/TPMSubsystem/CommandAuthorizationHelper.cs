// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Subsystems.TPMClient;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{


	public class CommandAuthorizationHelper : ICommandAuthorizationHelper
	{
		
		private ServerContext _ctx;
		private int _tpmSessionIdentifier;
		
		public CommandAuthorizationHelper (ServerContext ctx, int tpmSessionIdentifier)
		{
			_ctx = ctx;
			_tpmSessionIdentifier = tpmSessionIdentifier;
		}
		
		#region ICommandAuthorizationHelper implementation
		public AuthorizationInfo AuthorizeCommand (
			IAuthorizableCommand cmd, 
			AuthSessionNum authNum, 
			AuthHandle.AuthType? forceAuthType, 
			HMACKeyInfo keyInfo)
		{
			GenerateHMACRequest request = new GenerateHMACRequest (_ctx);
			request.TpmSessionIdentifier = _tpmSessionIdentifier;
			request.KeyInfo = keyInfo;
			
			GenerateHMACResponse response = request.TypedExecute ();
			Console.WriteLine("YEAAAHHH!!");
			return null;
		}
		
		#endregion

		public CommandAuthorizationHelper ()
		{
		}
	}
}
