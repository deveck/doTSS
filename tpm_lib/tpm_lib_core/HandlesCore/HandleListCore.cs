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
using Iaik.Utils.Serialization;
using System.Collections.Generic;
using System.IO;
using Iaik.Utils;
using Iaik.Tc.TPM.Library.Common.Handles;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Library.HandlesCore
{

	[TypedStreamSerializable("handle_list")]
	public class HandleListCore : HandleList, ITPMBlobReadable
	{
		/// <summary>
		/// Identifies the handles that are created for this handle list
		/// </summary>
		//private TPMResourceType _resourceType;
		
		public HandleListCore (TPMBlob blob, TPMResourceType resourceType)
		{
			//_resourceType = resourceType;
			ReadFromTpmBlob (blob);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			UInt16 handleCount = blob.ReadUInt16 ();
			
			for (int i = 0; i < handleCount; i++)
				_handles.Add (blob.ReadUInt32());
		}
		
		#endregion
		
		
	}
}
