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
