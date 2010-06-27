// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Locking;
using Iaik.Tc.TPM.Keystore;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;

namespace Iaik.Tc.TPM.Library.Common.KeyData
{

	/// <summary>
	/// Implemented by classes which swap in and swap out key contexts, 
	/// and load a given key hierachy
	/// </summary>
    /// <remarks>
    /// <para>There is exactly one key manager for each tpm device</para>
    /// <para>The IKeyManager is invoked on every execution of a command that requires a key handle and takes care that
    /// the required keys are loaded</para>
    /// <para>The IKeyManager is the only entity that executes the LoadKey command and with the IAuthHandleManager the only entity
    /// that executes SaveContext and LoadContext</para>
    /// </remarks>
	public interface IKeyManager
	{	
		
		/// <summary>
		/// Checks if the specified key is loaded, if not 
		/// the key hierachy is reconstructed and loaded
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="keyContext">Specifies the context in which an already loaded key can be used.
		/// If a key has already been loaded in another context it is not allowed to use this key</param>
		void LoadKey(string identifier, object keyContext, IKeyManagerHelper keymanagerHelper, ICommandAuthorizationHelper commandAuthHelper);
		
		/// <summary>
		/// Returns the tpm handle of the given key identifier,
		/// The key must be loaded before.
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="keyContext">Specifies the context in which an already loaded key can be used.
		/// If a key has already been loaded in another context it is not allowed to use this key
		/// </param>
		/// <returns></returns>
		KeyHandle IdentifierToHandle(string identifier, object keyContext, IKeyManagerHelper keymanagerHelper);
		
		/// <summary>
		/// Acquires an exclusive lock,
		/// to be sure that while swapping in some keys another thread cannot swap them out
		/// </summary>
		/// <returns></returns>
		LockContext AcquireLock();
		
		/// <summary>
		/// Ensures that a free key slot is available
		/// </summary>
		void EnsureFreeSlot();
	}
}
