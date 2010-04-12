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
		/// <param name="authNum">Authorization number (Auth1, Auth2)</param>
		/// <param name="forceAuthType">Lock to a specific authorization type (OSAP, OIAP), if null it is up to
		/// the framework which type to use (OIAP)
		/// </param>
		/// <param name="keyInfo">Contains information about the key to use for HMAC generation, this is transmitted to
		/// the client which requests the credentials from the user (if not cached) and generates the HMAC</param>
		/// <returns></returns>
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
			response.AssertResponse();
			
			Console.WriteLine("YEAAAHHH!!");
			return null;
		}
		
		#endregion
	}
}
