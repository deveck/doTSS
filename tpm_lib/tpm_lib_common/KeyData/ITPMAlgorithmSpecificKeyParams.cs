// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;

namespace Iaik.Tc.TPM.Library.Common.KeyData
{


	public interface ITPMAlgorithmSpecificKeyParams : ITypedStreamSerializable
	{
		/// <summary>
		/// Returns the bit key length
		/// </summary>
		uint KeyLength{get;}

		/// <summary>
		/// Byte size of the input blocks
		/// </summary>		
		uint InputBlockSize{get; }
		
		/// <summary>
		/// Byte size of the output blocks
		/// </summary>
	    uint OutputBlockSize{get;}
		
	}
}
