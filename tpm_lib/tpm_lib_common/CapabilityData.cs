
using System;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common
{


	public static class CapabilityData
	{
		public const string PARAM_TPM_VERSION_INFO = "tpm_version_info";
		public const string PARAM_PROP_PCR = "prop_pcr";
		public const string PARAM_PROP_DIR = "prop_dir";
		public const string PARAM_PROP_MANUFACTURER = "prop_manafacturer";
		public const string PARAM_PROP_KEYS = "prop_keys";
		public const string PARAM_PROP_MAX_AUTHSESS = "max_authsess";
		public const string PARAM_PROP_MAX_TRANSESS = "max_transess";
		public const string PARAM_PROP_MAX_KEYS = "max_keys";
		public const string PARAM_PROP_MAX_SESSIONS = "max_sessions";
		
		
		public enum TPMCapabilityArea : uint
		{
			/// <summary>
			/// Queries various values from the tpm
			/// </summary>
			TPM_CAP_PROPERTY = 0x00000005,
			

			/// <summary>
			/// Enumerates all handles of the specified type currently loaded in the tpm
			/// </summary>
			TPM_CAP_HANDLE = 0x00000014,
			
			/// <summary>
			/// Returns what the TPM currently supports
			/// </summary>
			TPM_CAP_VERSION_VAL	= 0x0000001A
		}
		
		/// <summary>
		/// SubCaps for TPM_CAP_PROPERTY
		/// </summary>
		public enum TPMSubCapProperty : uint
		{
			/// <summary>
			/// Returns the number of PCR registers supported by the tpm
			/// </summary>
			TPM_CAP_PROP_PCR = 0x000000101,
			
			/// <summary>
			/// Deprecated!. Returns the number of DIR, which is now fixed at 1
			/// </summary>
			[Obsolete("Deprecated in version 1.2 of TPM")]
			TPM_CAP_PROP_DIR = 0x000000102,
			
			/// <summary>
			/// Returns the vendor ID unique to each TPM manufacturer
			/// </summary>
			TPM_CAP_PROP_MANUFACTURER = 0x000000103,
			
			/// <summary>
			/// Returns the number of 2048-bit RSA keys that can be loaded. This may vary with time an circumstances
			/// </summary>
			TPM_CAP_PROP_KEYS = 0x000000104,
						
			/// <summary>
			/// The maximum number of loaded authorization sessions the TPM supports, varies with time
			/// </summary>
			TPM_CAP_PROP_MAX_AUTHSESS = 0x00000010D,
			
			/// <summary>
			/// The maximum number of loaded transport sessions the TPM supports, varies with time
			/// </summary>
			TPM_CAP_PROP_MAX_TRANSESS = 0x00000010E,
			
			/// <summary>
			/// The maximum number of 2048-bit RSA keys that the TPM can support. The number does not include the EK and the SRK.
			/// </summary>
			TPM_CAP_PROP_MAX_KEYS = 0x00000110,
			
			/// <summary>
			/// The maximum number of sessions the TPM supports
			/// </summary>
			TPM_CAP_PROP_MAX_SESSIONS = 0x00000011E
		}

		
		[TypedStreamSerializable(CapabilityData.PARAM_TPM_VERSION_INFO)]
		public class TPMCapVersionInfo : AutoStreamSerializable, ITypedParameter
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
			protected byte[] _tpmVendorId;
			/// <summary>
			/// The vendor id, unique to each tpm manufacturer
			/// </summary>
		 	public byte[] TpmVendorId
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
			
			public TPMCapVersionInfo (Stream src)
			{
				Read (src);
			}
			
			
		}
		
		[TypedStreamSerializable("TPMVersion")]
		public class TPMVersion : AutoStreamSerializable, ITypedParameter
		{
			[SerializeMe(0)]
			protected byte _major;
			
			public byte Major
			{
				get { return _major;}
			}
			
			[SerializeMe(1)]
			protected byte _minor;
			
			public byte Minor
			{
				get { return _minor;}
			}
			
			[SerializeMe(2)]
			protected byte _revMajor;
			
			public byte RevMajor
			{
				get { return _revMajor;}
			}
			
			[SerializeMe(3)]
			protected byte _revMinor;
			
			public byte RevMinor
			{
				get { return _revMinor;}
			}
			
			public TPMVersion ()
			{
			}
			
			public override void Write (Stream sink)
			{
				base.Write (sink);
			}

		}
	}
}
