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


// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Locking;

namespace Iaik.Tc.TPM.Library.Common.KeyData
{

	/// <summary>
	/// Encapsulates the key manager, because the keymanager is unique
	/// for each tpm and therefor needs some data that can not be provided by 
	/// the commands (endpoint, key context,...)
	/// </summary>
	public interface IKeyManagerHelper
	{
		/// <summary>
		/// Returns if the associated (client-) keystore contains a key with the specified identifier
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		bool ContainsIdentifier(string identifier);
	
		/// <summary>
		/// Looks for the specified key and returns its parent key
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		string FindParentKey(string identifier);
	
		/// <summary>
		/// Checks if the specified key is loaded, if not 
		/// the key hierachy is reconstructed and loaded
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="keyContext">Specifies the context in which an already loaded key can be used.
		/// If a key has already been loaded in another context it is not allowed to use this key</param>
		void LoadKey(string identifier);
		
		/// <summary>
		/// Returns the tpm handle of the given key identifier,
		/// The key must be loaded before.
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="keyContext">Specifies the context in which an already loaded key can be used.
		/// If a key has already been loaded in another context it is not allowed to use this key
		/// </param>
		/// <returns></returns>
		KeyHandle IdentifierToHandle(string identifier);
		
		/// <summary>
		/// Ensures that a free key slot is available
		/// </summary>
		void EnsureFreeSlot();
		
		/// <summary>
		/// Returns the key data for the specified identifier
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		byte[] GetKeyBlob(string identifier);
		
		/// <summary>
		/// Acquires an exclusive lock,
		/// to be sure that while swapping in some keys another thread cannot swap them out
		/// </summary>
		/// <returns></returns>
		LockContext AcquireLock();
	}
}
