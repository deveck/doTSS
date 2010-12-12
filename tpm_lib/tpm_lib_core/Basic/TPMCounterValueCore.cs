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


// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Basic;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Lowlevel.Data;
using System.IO;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Library.Basic
{

	/// <summary>
	/// Contains the counter value
	/// </summary>
	[TypedStreamSerializable("TPMCounterValue")]
	public class TPMCounterValueCore : TPMCounterValue, ITPMBlobReadable
	{

		public static TPMCounterValueCore CreateFromTPMBlob(TPMBlob blob)
		{
			TPMCounterValueCore counterValue = new TPMCounterValueCore();
			counterValue.ReadFromTpmBlob(blob);
			return counterValue;
		}
		
		private TPMCounterValueCore()
		{
		}
		
		public TPMCounterValueCore(Stream src)
			:base(src)
		{
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_structureTag = (TPMStructureTag)blob.ReadUInt16();
			_label = blob.ReadBytes(4);
			_counterValue = blob.ReadUInt32();
		}
		
		#endregion
	}
}
