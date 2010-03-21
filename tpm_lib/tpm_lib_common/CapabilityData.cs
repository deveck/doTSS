
using System;
using Iaik.Utils.Serialization;

namespace Iaik.Tc.TPM.Library.Common
{


	public static class CapabilityData
	{
		public enum TPMCapabilityArea : uint
		{
			/// <summary>
			/// Returns what the TPM currently supports
			/// 
			/// </summary>
			TPM_CAP_VERSION_VAL	= 0x0000001A
		}

		
		public class TPMCapVersionInfo : AutoStreamSerializable
		{

			[SerializeMe(0)]
			protected ushort _tag;

			/// <summary>
			/// Tag identifier, the upper nibble of the value must be null
			/// </summary>
			public ushort Tag
			{
				get { return _tag;}
			}
			
			
			[SerializeMe(1)]
			protected TPMVersion _version;
			
			/// <summary>
			/// The Version and revision
			/// </summary>
			public TPMVersion Version
			{
				get { return _version;}
			}
			
			
			[SerializeMe(2)]
			protected ushort _specLevel;
			
			/// <summary>
			/// A number indicating the level of ordinals supported
			/// </summary>
			public ushort SpecLevel
			{
				get { return _specLevel;}
			}
			
			
			[SerializeMe(3)]
			protected byte _errataRev;
			/// <summary>
			/// A number indicating the errate version of the specification
			/// </summary>
			public byte ErrataRev
			{
				get { return _errataRev;}
			}
				
			
			
			[SerializeMe(4)]
			protected byte _tpmVendorId;
			/// <summary>
			/// The vendor id, unique to each tpm manufacturer
			/// </summary>
		 	public byte TpmVendorId
			{
		 		get { return _tpmVendorId; }
			}

			
			[SerializeMe(5)]
			protected byte[] _vendorSpecific;
			/// <summary>
			/// Vendor specific information
			/// </summary>
			public byte[] VendorSpecific
			{
				get { return _vendorSpecific; }
			}
			
			protected TPMCapVersionInfo()
			{
			}
			
		
		}
		
		public class TPMVersion : AutoStreamSerializable
		{
			[SerializeMe(0)]
			protected byte _major;
			
			[SerializeMe(1)]
			protected byte _minor;
			
			[SerializeMe(2)]
			protected byte _revMajor;
			
			[SerializeMe(3)]
			protected byte _revMinor;
			
			protected TPMVersion()
			{
			}
		}
	}
}
