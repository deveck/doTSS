
using System;
using Iaik.Utils.Locking;

namespace Iaik.Tc.TPM.Library.Common.Handles.Authorization
{

	/// <summary>
	/// Implemented by backend specific classes that provide HMAC generation
	/// </summary>
	public interface ICommandAuthorizationHelper
	{
	
		/// <summary>
		/// Acquires an exclusive lock
		/// </summary>
		/// <returns>
		/// A <see cref="LockContext"/>
		/// </returns>
		ILockContext AcquireLock();
		
		/// <summary>
		/// Assures that the shared secret for the specified authorization handle has been
		/// calculated, if not it gets calculated
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="sessionNum"></param>
		/// <returns></returns>
		AuthHandle AssureOSAPSharedSecret(IAuthorizableCommand cmd, AuthSessionNum sessionNum);
	
		/// <summary>
		/// Returns the required authorization information for the specified command
		/// </summary>
		/// <param name="cmd">Specifies the command that needs authorization</param>
		/// <param name="authNum">Specifies which AuthNum is generated (Auth1, Auth2)</param>
		/// <param name="forceAuthType">If not null, forces the framework to use the specified Authentication method</param>
		/// <returns></returns>
		AuthorizationInfo[] AuthorizeCommand(IAuthorizableCommand cmd);
		
		/// <summary>
		/// Generates auth data from a tpm response, to compare to the 
		/// received auth data
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		AuthorizationInfo[] GenerateResponseAuthData(IAuthorizableCommand cmd);
		
		/// <summary>
		/// Extracts the AuthHandles and loads them
		/// </summary>
		/// <param name="authInfos">
		/// A <see cref="AuthorizationInfo[]"/>
		/// </param>
		void LoadAuthorizationHandles(params AuthorizationInfo[] authInfos);
		
		/// <summary>
		/// Loads the auth handles into the tpm
		/// </summary>
		/// <param name="authHandles">
		/// A <see cref="AuthHandle[]"/>
		/// </param>
		void LoadAuthorizationHandles(params AuthHandle[] authHandles);
		
		/// <summary>
		/// Extracts the AuthHandles and destroys them
		/// </summary>
		/// <param name="authInfos">
		/// A <see cref="AuthorizationInfo[]"/>
		/// </param>
		void DestroyAuthorizationHandles(IAuthorizableCommand cmd);
		
		/// <summary>
		/// Destroys the authorization handle
		/// </summary>
		/// <param name="handle">
		/// A <see cref="AuthHandle"/>
		/// </param>
		void DestroyAuthorizationHandle(IAuthorizableCommand cmd, AuthHandle handle);
		
		/// <summary>
		/// Extracts the AuthHandles and releases them
		/// </summary>
		/// <param name="authInfos">
		/// A <see cref="AuthorizationInfo[]"/>
		/// </param>
		void ReleaseAuthorizationHandles(IAuthorizableCommand cmd);
	
		
		/// <summary>
		/// Removes the authorization handles from the local auth manager
		/// </summary>
		/// <param name="cmd">
		/// A <see cref="IAuthorizableCommand"/>
		/// </param>
		void RemoveAuthorizationHandle(IAuthorizableCommand cmd);

        /// <summary>
        /// Removes the authorization handle
        /// </summary>
        /// <param name="handle">
        /// A <see cref="AuthHandle"/>
        /// </param>
        void RemoveAuthorizationHandle(IAuthorizableCommand cmd, AuthHandle handle);
	}
}
