// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Utils.Locking;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.KeyData;

namespace Iaik.Tc.TPM.Library.Commands
{
	public abstract class TPMCommand //: IDisposable
	{
		protected TPMProvider _tpmProvider = null;
		
		protected readonly UInt32 commandOrdinal_;
		protected readonly UInt32 commandTag_;
		protected  Parameters _params;
		
		/// <summary>
		/// Acquires locks that make sure that only the command that holds
		/// the lock is executing a critical section.
		/// 
		/// Critical sections are:
		///   * Context loading e.g.:
		/// 
		/// 		1. MasterCommand -> 
		/// 		2. AcquireLock -> 
		/// 		3. LoadContext1 -> 
		/// 		4. LoadContext2 -> 
		/// 		5. Execute command that requires context 1 and context 2 -> 
		/// 		6. Release lock
		/// 
		///   * Key loading e.g.:
		/// 		1. MasterCommand ->
		/// 		2. AcquireLock ->
		/// 		3. LoadKey ->
		/// 		4. Execute command that requires the key ->
		/// 		5. Release lock
		/// </summary>
		protected LockProvider _commandLockProvider;
		
		/// <summary>
		/// Manages key access/loading/swaping, 
		/// </summary>
		/// <remarks>
		/// To be sure that a key is available (loaded into the tpm) at a specific point follow the 
		/// instructions below:
		///
		///    * LoadKey (identifier)
		///       The Key manager reconstructs the key hierachy of the specified key and loads them one after
		///       another. Once the key with the specified identifier is loaded the method returns (no return value)
		///
		///	   * Acquire lock
		///    * Call keymanager.IdentifierToHandle(identifier)
		///	   * Insert the missing handle(s)
		///    * Run command
		///    * Release Lock 
		/// </remarks>
		protected IKeyManagerHelper _keyManager = null;
		
		/// <summary>
		/// Specifies the key context for this command,
		/// for further information <see cref="Iaik.Tc.TPM.Library.Common.KeyData.IKeyManager"/>
		/// </summary>
		protected object _keyContext = null;
		
		/// <summary>
		/// The response
		/// </summary>
		protected TPMBlob _responseBlob = null;
		
		protected TPMCommand(){}
		
		protected TPMCommand(UInt32 tag, UInt32 ordinal)
		{
			commandTag_ = tag;
			commandOrdinal_ = ordinal;
		}
		
		public void SetCommandLockProvider(LockProvider cmdLockProvider)
		{
			_commandLockProvider = cmdLockProvider;
		}
		
		public void SetKeyManager(IKeyManagerHelper keyManager)
		{
			_keyManager = keyManager;
		}
		
		public void SetKeyContext(object keyContext)
		{
			_keyContext = keyContext;
		}
		
//		private TPMCommand (UInt32 tag, UInt32 ordinal, Parameters param) : this(tag, ordinal)
//		{
//			_params = param;
//		}
//		
		public virtual void Init (Parameters param, TPMProvider tpmProvider)
		{
			_tpmProvider = tpmProvider;
			_params = param;
		}
		
		
		//public abstract void Process(Parameters param);
		//public abstract void Process();
		public abstract TPMCommandResponse Process();
		
		
		public virtual void Clear ()
		{
		}
		
			
	}
		
}
