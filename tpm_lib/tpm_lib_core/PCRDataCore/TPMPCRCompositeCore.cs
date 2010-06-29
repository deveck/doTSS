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
			_pcrValues = new byte[valueSize][];
			
			for(int i = 0; i<valueSize/20; i++)
				_pcrValues[i] = blob.ReadBytes(20);
			
		}
		
		#endregion
		
	}
}
