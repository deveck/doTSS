// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils.Hash;

namespace Iaik.Tc.TPM.Library.Common.PCRData
{

	[TypedStreamSerializable("TPMPCRInfo")]
	public class TPMPCRInfo : AutoStreamSerializable, ITypedParameter
	{
	
		/// <summary>
		/// Contains the selected PCR registers
		/// </summary>
		[SerializeMe(0)]
		protected TPMPCRSelection _pcrSelection;
		
		
//		protected byte[] DigestAtRelease
//		{
//			get
//			{
//				return new HashProvider().Hash(new HashByteDataProvider(_pcrSelection.PcrSelection.Data));
//			}
//		}
//		
//		protected byte[] DigestAtCreation
//		{
//			get
//			{
//				return DigestAtRelease;
//			}
//		}

		protected TPMPCRInfo ()
		{
		}
		
		public TPMPCRInfo(Stream src)
		{
			Read(src);
		}
	}
}
