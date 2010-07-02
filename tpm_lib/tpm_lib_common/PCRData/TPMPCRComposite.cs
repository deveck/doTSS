// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.PCRData
{

	[TypedStreamSerializable("TPMPCRComposite")]
	public class TPMPCRComposite : AutoStreamSerializable, ITypedParameter
	{
        [SerializeMe(0)]
		protected TPMPCRSelection _pcrSelection;
		
		public TPMPCRSelection PCRSelection
		{
			get{ return _pcrSelection; }
			set{ _pcrSelection = value; }
		}

        [SerializeMe(1)]
		protected byte[][] _pcrValues;
		
		public byte[][] PCRValues
		{
			get{ return _pcrValues; }
			set{ _pcrValues = value; }
		}

		public TPMPCRComposite ()
		{
		}

        public TPMPCRComposite(Stream src)
        {
            Read(src);
        }
	}
}
