// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.Storage
{

	[TypedStreamSerializable("TPMStoredData")]
	public class TPMStoredData : AutoStreamSerializable, ITypedParameter
	{

		/// <summary>
		/// This must be 1.1.0.0
		/// </summary>
		[SerializeMe(0)]
		protected CapabilityData.TPMVersion _versionStruct;
			
		/// <summary>
		/// Contains pcr information to which the data is bound to
		/// </summary>
		[SerializeMe(1)]
		protected byte[] _sealInfo;	
		
		/// <summary>
		/// Contains the confidential part of the data
		/// </summary>
		[SerializeMe(2)]
		protected byte[] _encData;
		

		protected TPMStoredData ()
		{
		}
		
		public TPMStoredData(Stream src)
		{
			Read(src);
		}
	}
}
