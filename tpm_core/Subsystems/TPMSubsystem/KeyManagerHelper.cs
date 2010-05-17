// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Tc.TPM.Context;
using Iaik.Utils.Locking;
using Iaik.Tc.TPM.Subsystems.TPMClient;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{


	public class KeyManagerHelper : IKeyManagerHelper
	{
		#region IKeyManagerHelper implementation
		public void LoadKey (string identifier)
		{
			_tpmContext.KeyManager.LoadKey(identifier, _tpmContext, this, _commandAuthHelper);
		}
		
		
		public KeyHandle IdentifierToHandle (string identifier)
		{
			return _tpmContext.KeyManager.IdentifierToHandle(identifier, _tpmContext, this);
		}
		
		
		public LockContext AcquireLock ()
		{
			return _tpmContext.KeyManager.AcquireLock();
		}
		
		public bool ContainsIdentifier (string identifier)
		{
			ContainsKeyRequest requestContainsKey = new ContainsKeyRequest(_ctx);
			requestContainsKey.Identifier = identifier;
			requestContainsKey.TpmSessionIdentifier = _tpmSessionIdentifier;
			ContainsKeyResponse responseContainsKey = requestContainsKey.TypedExecute();
			responseContainsKey.AssertResponse();
			
			return responseContainsKey.Contained;
		}
		
		
		public string FindParentKey (string identifier)
		{
			GetParentKeyRequest requestGetParent = new GetParentKeyRequest(_ctx);
			requestGetParent.Identifier = identifier;
			requestGetParent.TpmSessionIdentifier = _tpmSessionIdentifier;
			GetParentKeyResponse responseGetParent = requestGetParent.TypedExecute();
			responseGetParent.AssertResponse();
			
			return responseGetParent.ParentIdentifier;
		}
		
		public byte[] GetKeyBlob(string identifier)
		{
			GetKeyDataRequest requestGetKeyData = new GetKeyDataRequest(_ctx);
			requestGetKeyData.Identifier = identifier;
			requestGetKeyData.TpmSessionIdentifier = _tpmSessionIdentifier;
			GetKeyDataResponse responseGetKeyData = requestGetKeyData.TypedExecute();
			responseGetKeyData.AssertResponse();
			
			return responseGetKeyData.KeyData;
		}
		#endregion

	
		/// <summary>
		/// The associated server context
		/// </summary>
		private ServerContext _ctx;
		
		/// <summary>
		/// the associated TPM Context
		/// </summary>
		private TPMContext _tpmContext;

		/// <summary>
		/// Identifies the asociated tpm session
		/// </summary>
		private int _tpmSessionIdentifier;
		
		private ICommandAuthorizationHelper _commandAuthHelper;
		
		public KeyManagerHelper (ServerContext ctx, TPMContext tpmContext, int tpmSessionIdentifier, ICommandAuthorizationHelper commandAuthHelper)
		{
			_ctx = ctx;
			_tpmContext = tpmContext;
			_tpmSessionIdentifier = tpmSessionIdentifier;
			_commandAuthHelper = commandAuthHelper;
		}

	}
}
