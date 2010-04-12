
using System;

namespace Iaik.Tc.TPM.Library.Common.Handles.Authorization
{
	
	public enum AuthSessionNum
	{
		Auth1,
		Auth2
	}

	/// <summary>
	/// Implemented by commands (<see>TPMCommand</see>) that need authorization data
	/// </summary>
	public interface IAuthorizableCommand
	{
		/// <summary>
		/// Gets the SHA1 hash of the concatenated parameters
		/// </summary>
		byte[] Digest { get; }
		
		/// <summary>
		/// Sets the command authorization helper 
		/// </summary>
		/// <param name="commandAuthorizer"></param>
		void SetCommandAuthorizationHelper(ICommandAuthorizationHelper commandAuthorizer);
		
		/// <summary>
		/// Gets a predefined Auth1 or Auth2 handle. Retnrus null if no predefined handle is available.
		/// </summary>
		/// <param name="authSessionNum"></param>
		/// <returns></returns>
		AuthHandle GetAuthHandle(AuthSessionNum authSessionNum);
	}
}
