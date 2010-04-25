
using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Utils;
using Iaik.Utils.Serialization;

namespace Iaik.Tc.TPM.Library.Commands
{


	public static class CapabilityDataCore
	{
	
		[TypedStreamSerializable(CapabilityData.PARAM_TPM_VERSION_INFO)]
		public class TPMCapVersionInfoCore : CapabilityData.TPMCapVersionInfo, ITPMBlobReadable
		{
			public TPMCapVersionInfoCore (TPMBlob blob)
			{
				ReadFromTpmBlob(blob);
			}
			
			#region ITpmBlobReadable implementation
			public void ReadFromTpmBlob (TPMBlob blob)
			{
				/*uint responseSize = */blob.ReadUInt32 ();
				
				this._tag = blob.ReadUInt16 ();
				this._version = new TPMVersionCore (blob);
				this._specLevel = blob.ReadUInt16 ();
				this._errataRev = blob.ReadByte ();
				this._tpmVendorId = new byte[4];
				blob.Read (_tpmVendorId, 0, _tpmVendorId.Length);
				
				ushort vendorSize = blob.ReadUInt16 ();
				_vendorSpecific = blob.ReadBytes (vendorSize);				
				
			}
			
			#endregion
			
		}
		
		public class TPMVersionCore : CapabilityData.TPMVersion, ITPMBlobReadable, ITPMBlobWritable
		{
			/// <summary>
			/// Creates a default tpm v1.2 version structure
			/// </summary>
			/// <returns</returns>
			public static TPMVersionCore CreateVersion12 ()
			{
				CapabilityDataCore.TPMVersionCore version = new CapabilityDataCore.TPMVersionCore ();
				version._major = 0x01;
				version._minor = 0x02;
				version._revMajor = 0;
				version._revMinor = 0;
				
				return version;
			}
			
			public static TPMVersionCore CreateVersion11 ()
			{
				CapabilityDataCore.TPMVersionCore version = new CapabilityDataCore.TPMVersionCore ();
				version._major = 0x01;
				version._minor = 0x01;
				version._revMajor = 0;
				version._revMinor = 0;
				
				return version;
			}
			
			private TPMVersionCore ()
			{
			}
			
			internal TPMVersionCore (TPMBlob blob)
			{
				ReadFromTpmBlob(blob);
			}
			
			#region ITpmBlobReadable implementation
			public void ReadFromTpmBlob (TPMBlob blob)
			{
				_major = blob.ReadByte ();
				_minor = blob.ReadByte ();
				_revMajor = blob.ReadByte ();
				_revMinor = blob.ReadByte ();
			}
			
			#endregion
			#region ITPMBlobWritable implementation
			public void WriteToTpmBlob (TPMBlob blob)
			{
				blob.WriteByte (_major);
				blob.WriteByte (_minor);
				blob.WriteByte (_revMajor);
				blob.WriteByte (_revMinor);
			}
			
			#endregion
		}
	}
}
