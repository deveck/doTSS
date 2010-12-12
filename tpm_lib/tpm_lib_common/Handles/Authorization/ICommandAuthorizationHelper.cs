/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */



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
        /// Ensures that a free auth slot is available
        /// </summary>
        void EnsureFreeSlot();
	
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
