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
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;

namespace Iaik.Tc.TPM.Library.HandlesCore.Authorization
{


	[TypedStreamSerializable("AuthHandle")]
	public class AuthHandleCore : AuthHandle, ITPMBlobReadable
	{
		
		public AuthHandleCore(AuthType authType, TPMBlob blob)
		{
			_authType = authType;
			ReadFromTpmBlob(blob);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_authHandle = blob.ReadUInt32();
			
			_nonceEven = new byte[20];
			blob.Read(_nonceEven, 0, _nonceEven.Length);
			
			if(_authType == AuthHandle.AuthType.OSAP)
			{
				_nonceEvenOSAP = new byte[20];
				blob.Read(_nonceEvenOSAP, 0, _nonceEvenOSAP.Length);
			}
		}
		
		#endregion

		
	}
}
