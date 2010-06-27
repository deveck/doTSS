
using System;
using Iaik.Utils.Locking;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Library.Common.Handles.Authorization
{

	/// <summary>
	/// Implemented by classes which swap in/swap out and establish new authorization sessions for commands
	/// </summary>
    /// <remarks>
    /// <para>The IAuthHandleManager works completely transparently and is invoked for each command that needs authorization</para>
    /// <para>This is the only entity that executes TPM_OIAP and TPM_OSAP commands, and with the key manager the only entity
    /// that executes the SaveContext and LoadContext command.
    /// This commands should not be executed directly from the client.</para>
    /// </remarks>
	public interface IAuthHandleManager
	{
        /// <summary>
        /// Ensures that a free auth slot is available.
        /// Call this only from a locked context
        /// </summary>
        void EnsureFreeSlot();

		/// <summary>
		/// Acquires an exclusive auth handle manager lock
		/// </summary>
		/// <returns>
		/// A <see cref="LockContext"/>
		/// </returns>
		LockContext AcquireLock();
			
		/// <summary>
		/// Reserves the number of session slots this command requires
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
		void DestroyAuthHandles(IAuthorizableCommand cmd, params AuthHandle[] authHandles);
		
		/// <summary>
		/// MArks the specified authhandle as used so it does not get swapped out
		/// </summary>
		/// <param name="authHandle">
		/// A <see cref="AuthHandle"/>
		/// </param>
		void MarkAsUsed(IEnumerable<AuthHandle> authHandle);
		
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

        /// <summary>
        /// Removes the auth handles associated with the specified command
        /// from the local auth manager (they need to be already  destroyed on the tpm,
        /// otherwise use DestroyAuthHandles)
        /// </summary>
        /// <param name="authHandles">
        /// A <see cref="AuthHandle[]"/>
        /// </param>
        void RemoveAuthHandles(IAuthorizableCommand cmd, AuthHandle handle);
	}
}
