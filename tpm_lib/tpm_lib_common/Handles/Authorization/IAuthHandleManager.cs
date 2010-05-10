
using System;

namespace Iaik.Tc.TPM.Library.Common.Handles.Authorization
{

	/// <summary>
	/// Implemented by classes which swap in/swap out and establish new authorization sessions for commands
	/// </summary>
	public interface IAuthHandleManager
	{
		/// <summary>
		/// Reserves the number of session slots this command requires on the tpm.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="tpmSession"></param>
		void ReserveAuthHandleSlots(IAuthorizableCommand cmd);
		
		/// <summary>
		/// Forces the AuthHandleManager to use the specified AuthType
		/// </summary>
		/// <param name="forceAuthType"></param>
		/// <param name="cmd"></param>
		/// <returns>Returns an AuthHandle of the specified type</returns>
		AuthHandle GetAuthHandle(IAuthorizableCommand cmd, AuthSessionNum authSession);
		
		/// <summary>
		/// Releases the authhandles localy to make them usable by other commands
		/// </summary>
		/// <param name="authHandle">The AuthHandle to release</param>
		void ReleaseAuthHandles(IAuthorizableCommand cmd);
		
		/// <summary>
		/// Invalidates the auth handle on the tpm,
		/// after this call the handle is not usable anymore
		/// </summary>
		/// <param name="authHandle">The AuthHandle to release</param>
		void DestroyAuthHandles(IAuthorizableCommand cmd);
		
		/// <summary>
		/// Invalidates the auth handle on the tpm,
		/// after this call the handle is not usable anym
		/// </summary>
		/// <param name="authHandles">
		/// A <see cref="AuthHandle[]"/>
		/// </param>
		void DestroyAuthHandles(params AuthHandle[] authHandles);
		
		/// <summary>
		/// Loads the specified authorization handle
		/// </summary>
		/// <param name="authHandle"></param>
		void LoadAuthHandle(AuthHandle authHandle);
		
		/// <summary>
		/// Removes the auth handles associated with the specified command
		/// from the local auth manager (they need to be already  destroyed on the tpm,
		/// otherwise use DestroyAuthHandles)
		/// </summary>
		/// <param name="authHandles">
		/// A <see cref="AuthHandle[]"/>
		/// </param>
		void RemoveAuthHandles(IAuthorizableCommand cmd);
	}
}
