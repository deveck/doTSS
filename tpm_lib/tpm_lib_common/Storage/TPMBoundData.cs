// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.Storage
{

	/// <summary>
	/// Real data structure that is encrypted on bind operation.
	/// The TPM assumes that the decrypted data has this structure
	/// </summary>
	[TypedStreamSerializable("TPMBoundData")]
	public class TPMBoundData : AutoStreamSerializable, ITypedParameter
	{

		
		
		/// <summary>
		/// Must be 1.1.0.0
		/// </summary>
		[SerializeMe(0)]
		protected CapabilityData.TPMVersion _version;
		
		/// <summary>
		/// Shall be TPM_PT_BIND
		/// </summary>
		[SerializeMe(1)]
		protected TPMPayloadType _payloadType;
		
		/// <summary>
		/// The bound data
		/// </summary>
		[SerializeMe(2)]
		protected byte[] _payload;
		
		public TPMBoundData ()
		{
		}
		
		public TPMBoundData(Stream src)
		{
			Read(src);
		}
		
	}
}
