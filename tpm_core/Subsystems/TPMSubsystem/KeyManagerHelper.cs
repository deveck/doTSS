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
			_tpmContext.KeyManager.LoadKey(identifier, _tpmSessionIdentifier, this, _commandAuthHelper);
		}
		
		
		public KeyHandle IdentifierToHandle (string identifier)
		{
			return _tpmContext.KeyManager.IdentifierToHandle(identifier, _tpmSessionIdentifier, this);
		}
		
		
		public LockContext AcquireLock ()
		{
			return _tpmContext.KeyManager.AcquireLock();
		}
		
		public void EnsureFreeSlot()
		{
			_tpmContext.KeyManager.EnsureFreeSlot();
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
