// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.PCRData;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Library.PCRDataCore
{


	[TypedStreamSerializable("TPMPCRSelection")]
	public class TPMPCRSelectionCore : TPMPCRSelection, ITPMBlobWritable
	{
		

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
	}
}
