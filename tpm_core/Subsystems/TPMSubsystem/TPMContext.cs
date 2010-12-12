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
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common.KeyData;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{

	/// <summary>
	/// Combines all tpm provider related objects (tpm provider, Key storage, ...)
	/// </summary>
	public class TPMContext
	{
		private string _deviceName;
		
		/// <summary>
		/// Gets the configuration device name of this tpm device
		/// </summary>
		public string DeviceName
		{
			get { return _deviceName; }
		}
		
		private TPMWrapper _tpm;

		/// <summary>
		/// Gets access to the TPM device
		/// </summary>
		public TPMWrapper TPM
		{
			get { return _tpm;}
		}
		
		private IAuthHandleManager _authHandleManager;
		
		/// <summary>
		/// Manages and swaps the authorization handles as needed
		/// </summary>
		public IAuthHandleManager AuthHandleManager
		{
			get{ return _authHandleManager; }
		}
		
		private IKeyManager _keyManager;
		
		/// <summary>
		/// Manages and swapt the key handles as needed
		/// </summary>
		public IKeyManager KeyManager
		{
			get{ return _keyManager; }	
		}
		
		public TPMContext (string deviceName, TPMWrapper tpm)
		{
			_deviceName = deviceName;
			_tpm = tpm;
			_authHandleManager = new AuthHandleManager(this);
			_keyManager = new KeyManager(this);
		}
	}
}
