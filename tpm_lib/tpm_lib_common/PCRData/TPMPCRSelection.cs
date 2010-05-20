// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.Common.PCRData
{

	[TypedStreamSerializable("TPMPCRSelection")]
	public class TPMPCRSelection : AutoStreamSerializable, ITypedParameter
	{

		[SerializeMe(0)]
		protected BitMap _pcrSelection;
		
		protected TPMPCRSelection ()
		{
		}
		
		public TPMPCRSelection(Stream src)
		{
			Read(src);
		}
	}
}
