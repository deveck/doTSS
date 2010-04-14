
using System;

namespace Iaik.Tc.TPM.Library.Common.Handles.Authorization
{

	/// <summary>
	/// Implemented by classes 
	/// </summary>
	public interface IAuthHandleManager
	{
		/// <summary>
		/// Reserves the number of session slots this command requires on the tpm.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="tpmSession"></param>
		void ReserveAuthHandleSlots(IAuthorizableCommand cmd, object tpmSession);
		
		/// <summary>
		/// Forces the AuthHandleManager to use the specified AuthType
		/// </summary>
		/// <param name="forceAuthType"></param>
		/// <param name="cmd"></param>
		/// <returns>Returns an AuthHandle of the specified type</returns>
		AuthHandle GetAuthHandle(IAuthorizableCommand cmd, object tpmSession);
		
		/// <summary>
		/// Invalidates the auth handle on the tpm,
		/// after this call the handle is not usable anymore
		/// </summary>
		/// <param name="authHandle">The AuthHandle to release</param>
		void ReleaseAuthHandle(AuthHandle authHandle);
	}
}
