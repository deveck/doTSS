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
