// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Storage;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.TPM.Library.Commands;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Library.Storage
{

	/// <summary>
	/// Real data structure that is encrypted on bind operation.
	/// The TPM assumes that the decrypted data has this structure
	/// </summary>
	/// <remarks>
	/// The client requests the Prefix of the bound data (version, payloadtype)
	/// and only attaches the payload and encrypts this. 
	/// This is required because the client does not know how to serialize this
	/// for a tpm
	/// </remarks>
	[TypedStreamSerializable("TPMBoundData")]
	public class TPMBoundDataCore : TPMBoundData, ITPMBlobWritable
	{
		public static TPMBoundDataCore Encapsulate(byte[] payload)
		{
			TPMBoundDataCore boundData = new TPMBoundDataCore();
			boundData._version = CapabilityDataCore.TPMVersionCore.CreateVersion11();
			boundData._payloadType = TPMPayloadType.TPM_PT_BIND;
			boundData._payload = payload;
			return boundData;		
		}

		private TPMBoundDataCore ()
		{
		}
		
		public TPMBoundDataCore(Stream src)
			:base(src)
		{
		}
		#region ITPMBlobWritable implementation
		public void WriteToTpmBlob (TPMBlob blob)
		{
			((ITPMBlobWritable)_version).WriteToTpmBlob(blob);
			blob.WriteByte((byte)_payloadType);
			blob.Write(_payload, 0, _payload.Length);
		}
		
		#endregion
	}
}
