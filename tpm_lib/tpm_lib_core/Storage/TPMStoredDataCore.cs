// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Storage;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Commands;

namespace Iaik.Tc.TPM.Library.Storage
{


	[TypedStreamSerializable("TPMStoredData")]
	public class TPMStoredDataCore : TPMStoredData, ITPMBlobWritable, ITPMBlobReadable
	{
	
		public static TPMStoredDataCore CreateFromTPMBlob(TPMBlob blob)
		{
			TPMStoredDataCore storedData = new TPMStoredDataCore();
			storedData.ReadFromTpmBlob(blob);
			return storedData;
		}
		
		public static TPMStoredDataCore CreateFromBytes(byte[] data)
		{
			using(MemoryStream src = new MemoryStream(data))
			{
				return new TPMStoredDataCore(src);
			}		
		}
				

		private TPMStoredDataCore ()
		{
		}
		
		public TPMStoredDataCore(Stream src)
		 :base(src)
		{
		}
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_versionStruct = CapabilityDataCore.TPMVersionCore.CreateFromTPMBlob(blob);
			
			_sealInfo = new byte[blob.ReadUInt32()];
			blob.Read(_sealInfo, 0, _sealInfo.Length);
			
			_encData = new byte[blob.ReadUInt32()];
			blob.Read(_encData, 0, _encData.Length);
		}
		
		#endregion
		
		#region ITPMBlobWritable implementation
		public void WriteToTpmBlob (TPMBlob blob)
		{
			((ITPMBlobWritable)_versionStruct).WriteToTpmBlob(blob);
			blob.WriteUInt32((uint)_sealInfo.Length);
			blob.Write(_sealInfo,0,_sealInfo.Length);
			blob.WriteUInt32((uint)_encData.Length);
			blob.Write(_encData, 0, _encData.Length);
		}
		
		#endregion
	}
}
