// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Library.Common.PCRData
{

	[TypedStreamSerializable("TPMPCRComposite")]
	public class TPMPCRCompositeCore : TPMPCRComposite, ITPMBlobWritable
	{
		#region ITPMBlobWritable implementation
		public void WriteToTpmBlob (TPMBlob blob)
		{
			((ITPMBlobWritable)_pcrSelection).WriteToTpmBlob(blob);
			blob.WriteUInt32((uint)_pcrValues.Length * 20);
			
			foreach(byte[] pcrValue in _pcrValues)
				blob.Write(pcrValue,0, pcrValue.Length);
		}
		
		#endregion
		
	}
}
