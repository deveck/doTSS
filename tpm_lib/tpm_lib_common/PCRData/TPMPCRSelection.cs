// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Library.Common.PCRData
{

	[TypedStreamSerializable("TPMPCRSelection")]
	public class TPMPCRSelection : AutoStreamSerializable, ITypedParameter
	{

		/// <summary>
		/// Creates a new TPMPCRSelection structure with the given number of pcrslots
		/// </summary>
		/// <param name="pcrCount"></param>
		/// <returns></returns>
		public static TPMPCRSelection CreatePCRSelection(uint pcrCount)
		{
			TPMPCRSelection selection = new TPMPCRSelection();
			selection._pcrSelection = new BitMap((int)pcrCount);
			return selection;
		}
		
		/// <summary>
		/// Contains the selected PCR registers
		/// </summary>
		[SerializeMe(0)]
		protected BitMap _pcrSelection;
		
		/// <summary>
		/// Gets the pcr selection
		/// </summary>
		public BitMap PcrSelection
		{
			get{ return _pcrSelection; }
		}


        /// <summary>
        /// Returns the selected PCRs
        /// </summary>
        public IList<int> SelectedPCRs
        {
            get
            {
                List<int> selectedPCRs = new List<int>();

                for(int i = 0; i<_pcrSelection.BitCount; i++)
                {
                    if (_pcrSelection.GetBit(i))
                        selectedPCRs.Add(i);
                }

                return selectedPCRs;
            }
        }

		protected TPMPCRSelection ()
		{
		}
		
		
		public TPMPCRSelection(Stream src)
		{
			Read(src);
		}
	}
}
