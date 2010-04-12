
using System;

namespace Iaik.Tc.TPM.Library.Common.Handles.Authorization
{

	/// <summary>
	/// Implemented by backend specific classes that provide HMAC generation
	/// </summary>
	public interface ICommandAuthorizationHelper
	{
		/// <summary>
		/// Returns the required authorization information for the specified command
		/// </summary>
		/// <param name="cmd">Specifies the command that needs authorization</param>
		/// <param name="authNum">Specifies which AuthNum is generated (Auth1, Auth2)</param>
		/// <param name="forceAuthType">If not null, forces the framework to use the specified Authentication method</param>
		/// <returns></returns>
		AuthorizationInfo AuthorizeCommand(
			IAuthorizableCommand cmd, 
			AuthSessionNum authNum, 
			AuthHandle.AuthType? forceAuthType,
			HMACKeyInfo keyInfo
			);			
	}
}
