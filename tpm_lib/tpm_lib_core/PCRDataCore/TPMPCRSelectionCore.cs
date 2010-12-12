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
using Iaik.Tc.TPM.Library.Common.PCRData;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.PCRDataCore
{


	[TypedStreamSerializable("TPMPCRSelection")]
	public class TPMPCRSelectionCore : TPMPCRSelection, ITPMBlobWritable, ITPMBlobReadable
	{
		public static TPMPCRSelectionCore CreateFromTPMBlob(TPMBlob blob)
		{
			TPMPCRSelectionCore pcrSelection = new TPMPCRSelectionCore();
			pcrSelection.ReadFromTpmBlob(blob);
			return pcrSelection;
		}

		private TPMPCRSelectionCore()
		{
		}

		public TPMPCRSelectionCore(TPMPCRSelection pcrSelection)
		{
			_pcrSelection = pcrSelection.PcrSelection;
		}

		public TPMPCRSelectionCore (Stream src)
			:base(src)
		{
		}
		
		#region ITPMBlobWritable implementation
		public void WriteToTpmBlob (TPMBlob blob)
		{
			blob.WriteUInt16((ushort)_pcrSelection.Data.Length);
			blob.Write(_pcrSelection.Data, 0, _pcrSelection.Data.Length);
		}
		
		#endregion
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			uint size = blob.ReadUInt16();
			byte[] selectionBits = new byte[size];
			blob.Read(selectionBits, 0, (int)size);
			_pcrSelection = new BitMap(selectionBits);
			
		}
		
		#endregion
	}
}
