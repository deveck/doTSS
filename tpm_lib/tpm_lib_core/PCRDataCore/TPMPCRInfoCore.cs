// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.PCRData;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Lowlevel.Data;
using System.IO;
using System.Collections.Generic;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Hash;

namespace Iaik.Tc.TPM.Library.PCRDataCore
{

	[TypedStreamSerializable("TPMPCRInfo")]
	public class TPMPCRInfoCore : TPMPCRInfo, ITPMBlobWritable, ITPMBlobReadable
	{
		public delegate byte[] GetPCRValueDelegate(uint pcrNum);
		
		public static TPMPCRInfoCore CreateFromTPMBlob(TPMBlob blob)
		{
			TPMPCRInfoCore pcrInfo = new TPMPCRInfoCore();
			pcrInfo.ReadFromTpmBlob(blob);
			return pcrInfo;	
		}
		
		private byte[] _digest = null;
		
		public byte[] DigestAtRelease
		{
			get{ return _digest; }
		}
		
		public byte[] DigestAtCreation
		{
			get{ return new byte[20]; }
		}
		

		private TPMPCRInfoCore()
		{
		}

		public TPMPCRInfoCore(TPMPCRSelection pcrSelection)
		{
			_pcrSelection = pcrSelection;
		}

		public TPMPCRInfoCore(Stream src)
			:base(src)
		{			
		}
		
		public void CalculateDigests(GetPCRValueDelegate getPCRValue)
		{
			List<byte[]> pcrValues = new List<byte[]>();
			
			for(uint i = 0; i<_pcrSelection.PcrSelection.BitCount; i++)
			{
				if(_pcrSelection.PcrSelection.GetBit((int)i))
				{
					pcrValues.Add(getPCRValue(i));
				}
			}
						
			TPMPCRCompositeCore composite = new TPMPCRCompositeCore();			
			composite.PCRSelection = _pcrSelection;
			composite.PCRValues = pcrValues.ToArray();
			
			_digest = new HashProvider().Hash(new HashTPMBlobWritableDataProvider(composite));
		}

		public override void Read (Stream src)
		{
			base.Read(src);
		}


		#region ITPMBlobWritable implementation
		public void WriteToTpmBlob (TPMBlob blob)
		{
		
			((ITPMBlobWritable)_pcrSelection).WriteToTpmBlob(blob);
			
			byte[] digestAtRelease = DigestAtRelease;
			byte[] digestAtCreation = DigestAtCreation;
			
			if(digestAtRelease.Length != 20 || digestAtCreation.Length != 20)
				throw new ArgumentException("Digest length of TPMPCRInfo != 20");
				
			blob.Write(digestAtRelease,0, digestAtRelease.Length);
			blob.Write(digestAtCreation,0,digestAtCreation.Length);
		}
		
		#endregion
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_pcrSelection = TPMPCRSelectionCore.CreateFromTPMBlob(blob);
			byte[] digestAtRelease = new byte[20];
			byte[] digestAtCreation = new byte[20];
			
			blob.Read(digestAtRelease, 0, 20);
			blob.Read(digestAtCreation, 0, 20);
		}
		
		#endregion
				
								

	}
}
