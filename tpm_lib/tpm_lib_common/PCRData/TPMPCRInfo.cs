// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.PCRData
{

	[TypedStreamSerializable("TPMPCRInfo")]
	public class TPMPCRInfo : AutoStreamSerializable, ITypedParameter
	{
		
		protected TPMPCRSelection _pcrSelection;
		

		protected TPMPCRInfo (TPMPCRSelection pcrSelection)
		{
			_pcrSelection = pcrSelection;
		}
		
		public TPMPCRInfo(Stream src)
		{
			Read(src);
		}
	}
}
