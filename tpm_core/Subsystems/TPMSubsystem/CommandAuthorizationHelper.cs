// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Subsystems.TPMClient;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{


	public class CommandAuthorizationHelper : ICommandAuthorizationHelper
	{
		
		/// <summary>
		/// The EndpointContext this CommandAuthorizationHelper belongs to
		/// </summary>
		private ServerContext _ctx;
		
		/// <summary>
		/// The TPM session identifier this CommandAuthorizationHelper belongs to.
		/// </summary>
		private int _tpmSessionIdentifier;
		
		public CommandAuthorizationHelper (ServerContext ctx, int tpmSessionIdentifier)
		{
			_ctx = ctx;
			_tpmSessionIdentifier = tpmSessionIdentifier;
		}
		
		#region ICommandAuthorizationHelper implementation
		
		/// <summary>
		/// Authorizes the command and returns the necessary authorization info
		/// </summary>
		/// <param name="cmd">Command to authorize</param>
		/// <returns></returns>
		public AuthorizationInfo[] AuthorizeCommand (IAuthorizableCommand cmd)
		{
			
			
			
			List<AuthorizationInfo> authorizationInfos = new List<AuthorizationInfo>();
			
			foreach(AuthSessionNum authSessionNum in new AuthSessionNum[]{AuthSessionNum.Auth1, AuthSessionNum.Auth2})
			{
				HMACKeyInfo keyInfo = cmd.GetKeyInfo(authSessionNum);
				
				if(keyInfo == null)
					continue;

				GenerateHMACRequest request = new GenerateHMACRequest (_ctx);
				request.TpmSessionIdentifier = _tpmSessionIdentifier;
				request.KeyInfo = keyInfo;
			
				GenerateHMACResponse response = request.TypedExecute ();
				response.AssertResponse();
				
				//TODO: Add new AuthorizationInfo
			}
			
			return authorizationInfos.ToArray();			
		}
		
		#endregion
	}
}
