
using System;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;
using Iaik.Tc.TPM.Library.Common.KeyData;

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
		/// Gets the SHA1 hash of the concatenated response parameters
		/// </summary>
		byte[] ResponseDigest { get; }
		
		IKeyManagerHelper KeyManager{get;}
		
		ICommandAuthorizationHelper CommandAuthHelper{get;}
		
		/// <summary>
		/// Gets informations about the received auth data
		/// </summary>
		ResponseAuthHandleInfo[] ResponseAuthHandleInfos{ get; }
	
		/// <summary>
		/// Gets information about the current auth handles in use
		/// </summary>
		AuthorizationInfo[] AuthorizationInfos{get;}
		
		/// <summary>
		/// Gets a handle needed by the authorization process (e.g. key handle for osap)
		/// </summary>
		/// <param name="authSessionNum"></param>
		/// <returns>Null or handle identifier</returns>
		string GetHandle(AuthSessionNum authSessionNum);
		
		/// <summary>
		/// Gets the EntityType for the specified auth session
		/// </summary>
		/// <remarks>
		/// This is required for e.g. OSAP.
		/// The EntityTypeMSB is selected by the caller itself
		/// </remarks>
		/// <param name="austhSessionNum"></param>
		/// <returns></returns>
		TPMEntityTypeLSB GetEntityType(AuthSessionNum austhSessionNum);
		
		/// <summary>
		/// Checks if the specified auth type is supported by the command
		/// </summary>
		/// <param name="authType"></param>
		/// <returns></returns>
		bool SupportsAuthType(AuthHandle.AuthType authType);
		
		/// <summary>
		/// Returns the HMACKeyInfo needed for the specified authSessionNum.
		/// Returns null if the specified AuthSessionNum is not required
		/// </summary>
		/// <param name="authType"></param>
		/// <returns></returns>
		HMACKeyInfo GetKeyInfo(AuthSessionNum authSessionNum);
		
		
	}
}
