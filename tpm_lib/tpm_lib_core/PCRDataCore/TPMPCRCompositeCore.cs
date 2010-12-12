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
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.PCRDataCore;

namespace Iaik.Tc.TPM.Library.Common.PCRData
{

	[TypedStreamSerializable("TPMPCRComposite")]
	public class TPMPCRCompositeCore : TPMPCRComposite, ITPMBlobWritable, ITPMBlobReadable
	{
		public static TPMPCRCompositeCore CreateFromTPMBlob(TPMBlob blob)
		{
			TPMPCRCompositeCore pcrComposite = new TPMPCRCompositeCore();
			pcrComposite.ReadFromTpmBlob(blob);
			return pcrComposite;
		}
	
		#region ITPMBlobWritable implementation
		public void WriteToTpmBlob (TPMBlob blob)
		{
			((ITPMBlobWritable)_pcrSelection).WriteToTpmBlob(blob);
			blob.WriteUInt32((uint)_pcrValues.Length * 20);
			
			foreach(byte[] pcrValue in _pcrValues)
				blob.Write(pcrValue,0, pcrValue.Length);
		}
		
		#endregion
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_pcrSelection = TPMPCRSelectionCore.CreateFromTPMBlob(blob);
			
			uint valueSize = blob.ReadUInt32();
			_pcrValues = new byte[valueSize/20][];
			
			for(int i = 0; i<valueSize/20; i++)
				_pcrValues[i] = blob.ReadBytes(20);
			
		}
		
		#endregion
		
	}
}
