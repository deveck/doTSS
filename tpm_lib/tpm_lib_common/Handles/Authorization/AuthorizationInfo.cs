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

namespace Iaik.Tc.TPM.Library.Common.Handles.Authorization
{


	public class AuthorizationInfo
	{
		private AuthHandle _authHandle;
		
		/// <summary>
		/// Associated authorization handle
		/// </summary>
		public AuthHandle Handle
		{
			get { return _authHandle; }
		}
		
		
		private byte[] _authData;
		
		public byte[] AuthData
		{
			get { return _authData;}
		}
		
		private bool _continueAuthSession;
		
		public bool ContinueAuthSession
		{
			get { return _continueAuthSession;}
		}
		
		public AuthorizationInfo (AuthHandle authHandle, bool continueAuthSession, byte[] authData)
		{
			_authHandle = authHandle;
			_authData = authData;
			_continueAuthSession = continueAuthSession;
		}
	}
}
