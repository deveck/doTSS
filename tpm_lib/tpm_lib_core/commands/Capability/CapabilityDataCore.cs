
using System;
using Iaik.Tc.Tpm.library.common;
using Iaik.Tc.Tpm.lowlevel.data;
using Iaik.Utils;

namespace Iaik.Tc.Tpm.library.commands
{


	public static class CapabilityDataCore
	{
	
		
		public class TPMCapVersionInfoCore : CapabilityData.TPMCapVersionInfo, ITpmBlobReadable
		{
			public TPMCapVersionInfoCore(TpmBlob blob)
			{
				Read(blob);
			}
			
			#region ITpmBlobReadable implementation
			public void ReadFromTpmBlob (TpmBlob blob)
			{
				this._tag = blob.ReadUInt16 ();
				this._version = new TPMVersionCore (blob);
				this._specLevel = blob.ReadUInt16 ();
				this._errataRev = blob.ReadByte ();
				this._tpmVendorId = blob.ReadByte ();
				
				ushort vendorSize = blob.ReadUInt16 ();
				_vendorSpecific = blob.ReadBytes (vendorSize);				
				
			}
			
			#endregion
			
		}
		
		public class TPMVersionCore : CapabilityData.TPMVersion, ITpmBlobReadable
		{
			internal TPMVersionCore(TpmBlob blob)
			{
				Read(blob);
			}
			
			#region ITpmBlobReadable implementation
			public void ReadFromTpmBlob (TpmBlob blob)
			{
				_major = blob.ReadByte ();
				_minor = blob.ReadByte ();
				_revMajor = blob.ReadByte ();
				_revMinor = blob.ReadByte ();
			}
			
			#endregion
		}
	}
}
