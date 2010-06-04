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
using log4net;

namespace Iaik.Tc.TPM.Library.Commands
{
	public abstract class TPMCommand //: IDisposable
	{
		/// <summary>
		/// Logger for debuggins purpose
		/// </summary>
		protected ILog _log = LogManager.GetLogger("TPMCommand");
		
		/// <summary>
		/// Response paramters only used for debugging purpose, do not need to be used
		/// </summary>
		protected Parameters _responseParameters = null;
		
		protected TPMProvider _tpmProvider = null;
		protected TPMWrapper _tpmWrapper = null;
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
		
		public IKeyManagerHelper KeyManager
		{
			get{ return _keyManager; }
		}
	
		protected ICommandAuthorizationHelper _commandAuthHelper;
		
		/// <summary>
		/// Gets the command authorization helper
		/// </summary>
		public ICommandAuthorizationHelper CommandAuthHelper 
		{ 
			get{ return _commandAuthHelper;}
		}
		
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
		
		/// <summary>
		/// Sets the command authorization helper 
		/// </summary>
		/// <param name="commandAuthorizer"></param>
		public void SetCommandAuthorizationHelper(ICommandAuthorizationHelper commandAuthorizer)
		{
			_commandAuthHelper = commandAuthorizer;
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
		public virtual void Init (Parameters param, TPMProvider tpmProvider, TPMWrapper tpmWrapper)
		{
			_tpmProvider = tpmProvider;
			_params = param;
			_tpmWrapper = tpmWrapper;
		}
		
		
		//public abstract void Process(Parameters param);
		//public abstract void Process();
		public abstract TPMCommandResponse Process();

		/// <summary>
		/// Transmits the specified request blob
		/// </summary>
		/// <param name="requestBlob"></param>
		/// <returns></returns>
		protected virtual TPMBlob TransmitMe(TPMBlob requestBlob)
		{
			lock(_tpmProvider)
			{
				try
				{
					_log.DebugFormat("Processing {0}", this);
					_log.DebugFormat("BeforeExecution: {0}", GetCommandInternalsBeforeExecute());
					return _tpmProvider.TransmitAndCheck(requestBlob);					
				}
				catch(Exception)
				{
					//If the request was not successful destroy all auth handles,
					//before releasing the tpmProvider lock
					throw;
				}
				finally
				{
					_log.DebugFormat("Processed {0}", this);
				}
			}
		}
		
		public virtual void Clear ()
		{
		}
		

		/// <summary>
		/// Gets a string representation of the internal command parameters for debugging purpose
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public virtual string GetCommandInternalsBeforeExecute()
		{
			if(_params == null)
				return "<null>";
			else
				return _params.ToString();
		}
		
		/// <summary>
		/// Gets a string representation of the internal command parameters for debugging purpose
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public virtual string GetCommandInternalsAfterExecute()
		{
			if(_responseParameters == null)
				return "<null>";
			else
				return _responseParameters.ToString();
		}

	}
		
}
