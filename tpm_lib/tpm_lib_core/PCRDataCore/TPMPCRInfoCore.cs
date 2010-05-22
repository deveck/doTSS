// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.PCRData;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Lowlevel.Data;
using System.IO;

namespace Iaik.Tc.TPM.Library.PCRDataCore
{

	[TypedStreamSerializable("TPMPCRInfo")]
	public class TPMPCRInfoCore : TPMPCRInfo, ITPMBlobWritable
	{
		

		public TPMPCRInfoCore(TPMPCRSelection pcrSelection)
		{
			_pcrSelection = pcrSelection;
		}

		public TPMPCRInfoCore(Stream src)
			:base(src)
		{			
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
				
								

	}
}
