
using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.Commands
{


	public static class CapabilityDataCore
	{
	
		
		public class TPMCapVersionInfoCore : CapabilityData.TPMCapVersionInfo, ITPMBlobReadable
		{
			public TPMCapVersionInfoCore(TPMBlob blob)
			{
				Read(blob);
			}
			
			#region ITpmBlobReadable implementation
			public void ReadFromTpmBlob (TPMBlob blob)
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
		
		public class TPMVersionCore : CapabilityData.TPMVersion, ITPMBlobReadable
		{
			internal TPMVersionCore(TPMBlob blob)
			{
				Read(blob);
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
		}
	}
}
