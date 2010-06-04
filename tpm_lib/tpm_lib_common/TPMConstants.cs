
using System;

namespace Iaik.Tc.TPM.Library.Common
{

	/// <summary>
	/// Defines the types of algorithms that may be supported by the tpm
	/// </summary>
	public enum TPMAlgorithmId : uint
	{
		/// <summary>
		/// RSA algorithm
		/// </summary>
		TPM_ALG_RSA = 0x01,
		
		/// <summary>
		/// SHA1 Algorithm
		/// </summary>
		TPM_ALG_SHA = 0x04,
		
		/// <summary>
		/// RFC2104 HMAC algorithm
		/// </summary>
		TPM_ALG_HMAC = 0x05,
		
		/// <summary>
		/// AES algorithm, keysize 128 bit
		/// </summary>
		TPM_ALG_AES128 = 0x06,
		
		/// <summary>
		/// XOR algorithm using MGF1 to create a string with the size of the encrypted block
		/// </summary>
		TPM_ALG_MGF1 = 0x07,
		
		/// <summary>
		/// AES algorithm, keysize 192 bit
		/// </summary>
		TPM_ALG_AES192 = 0x08,
		
		/// <summary>
		/// AES algorithm, keysize 256 bit
		/// </summary>
		TPM_ALG_AES256 = 0x09,
		
		/// <summary>
		/// XOR using rolling nonces
		/// </summary>
		TPM_ALG_XOR = 0x0A
		
	}
	
	
	/// <summary>
	/// Possible encryption schemes
	/// </summary>
	public enum TPMEncScheme : ushort
	{
		TPM_ES_NONE = 0x01,
		TPM_ES_RSAESPKCSv15 = 0x02,
		TPM_ES_RSAESOAEP_SHA1_MGF1 = 0x03,
		TPM_ES_SYM_CTR = 0x04,
		TPM_ES_SYM_OFB = 0x05
	}
	
	/// <summary>
	/// Possible signature schemes
	/// </summary>
	public enum TPMSigScheme : ushort
	{
		TPM_SS_NONE = 0x01,
		TPM_SS_RSASSAPKCS1v15_SHA1 = 0x02,
		TPM_SS_RSASSAPKCS1v15_DER = 0x03,
		TPM_SS_RSASSAPKCS1v15 = 0x04
	}
	
	/// <summary>
	/// Specifies the different resource types supported by the tpm
	/// </summary>
	public enum TPMResourceType : uint
	{
		/// <summary>
		/// Key handle
		/// </summary>
		TPM_RT_KEY = 0x01,
		
		/// <summary>
		/// Authorization handle
		/// </summary>
		TPM_RT_AUTH = 0x02,
		
		/// <summary>
		/// Reserved for hashes
		/// </summary>
		TPM_RT_HASH = 0x03,
		
		/// <summary>
		/// Handle for transport sessions
		/// </summary>
		TPM_RT_TRANS = 0x04,
		
		/// <summary>
		/// Resource wrapped and held outside the tpm
		/// </summary>
		TPM_RT_CONTEXT = 0x05,
		
		/// <summary>
		/// Reserved for counters
		/// </summary>
		TPM_RT_COUNTER = 0x06,
		
		/// <summary>
		/// Delegate rows
		/// </summary>
		TPM_RT_DELEGATE = 0x07,
		
		/// <summary>
		/// Value is a DAA TPM specific blob
		/// </summary>
		TPM_RT_DAA_TPM = 0x08,
		
		/// <summary>
		/// Value is a DAA V0 parameter
		/// </summary>
		TPM_RT_DAA_V0 = 0x09,
		
		/// <summary>
		/// Value is a DAA V1 parameter
		/// </summary>
		TPM_RT_DAA_V1 = 0x0A
	}
	
	/// <summary>
	/// Defines the possible types of keys
	/// </summary>
	public enum TPMKeyUsage : ushort
	{
		/// <summary>
		/// Indicates a signing key
		/// </summary>
		TPM_KEY_SIGNING = 0x10,
		
		/// <summary>
		/// Indicates a storage key
		/// </summary>
		TPM_KEY_STORAGE = 0x11,
		
		/// <summary>
		/// Indicates an identity key
		/// </summary>
		TPM_KEY_IDENTITY = 0x12,
		
		/// <summary>
		/// Indiates an ephemeral key that is used during ChangeAuthAsym process only
		/// </summary>
		TPM_KEY_AUTHCHANGE = 0x13,
		
		/// <summary>
		/// Indicates a key that can be used for TPM_Bind and TPM_UnBind
		/// </summary>
		TPM_KEY_BIND = 0x14,
		
		/// <summary>
		/// Indicates a key that can be used for signing and binding operations
		/// </summary>
		TPM_KEY_LEGACY = 0x15,
		
		/// <summary>
		/// Indicates a key in use for TPM_MigrateKey
		/// </summary>
		TPM_KEY_MIGRATE = 0x16
	}
	
	/// <summary>
	/// Defines flags that can be attached to keys
	/// </summary>
	[Flags()]
	public enum TPMKeyFlags : uint
	{
		/// <summary>
		/// No Flags
		/// </summary>
		None = 0x0,
		
		/// <summary>
		/// Indicates use of redirected output
		/// </summary>
		Redirection = 0x01,
		
		/// <summary>
		/// Indicates that the key is migratable
		/// </summary>
		Migratable = 0x02,
		
		/// <summary>
		/// Indicates that the key MUST be unloaded upon execution of TPM_STARTUP(ST_Clear)
		/// </summary>
		IsVolatile = 0x04,
		
		/// <summary>
		/// If set the TPM MUST NOT check digestAtRelease or localityAtRelease for commands that use the 
		/// public portion of the key like TPM_GetPubKey
		/// 
		/// If not set the TPM MUST check digestAtRelease and localityAtRelease for commands thet use the public
		/// portion of the key
		/// </summary>
		PcrIgnoredOnRead = 0x08,
		
		/// <summary>
		/// Indicates that the key is under control of a migratable authority	
		/// </summary>
		MigrateAuthority = 0x10
	}
	
	/// <summary>
	/// Indicates to the TPM when authorization sessions for an entity are required
	/// </summary>
	public enum TPMAuthDataUsage : byte
	{
		/// <summary>
		/// Usage of the key withoug authorization is permitted
		/// </summary>
		TPM_AUTH_NEVER = 0x00,
		
		/// <summary>
		/// Indicates that on each sage of the key the authorization must be performed
		/// </summary>
		TPM_AUTH_ALWAYS = 0x01,
		
		/// <summary>
		/// Indicates that on commands that require the TPM to use the private portion of the key
		/// the authorization must be performed
		/// </summary>
		TPM_AUTH_PRIV_USE_ONLY = 0x03
	}
	
	/// <summary>
	/// Identifies the protocol in use
	/// </summary>
	public enum TPMProtocolId : ushort
	{
		TPM_PID_OIAP = 0x01,
		TPM_PID_OSAP = 0x02,
		TPM_PID_ADIP = 0x03,
		TPM_PID_ADCP = 0x04,
		TPM_PID_OWNER = 0x05,
		TPM_PID_DSAP = 0x06,
		TPM_PID_TRANSPORT = 0x07
	}
	
	
	/// <summary>
	/// Specifies the available ADIP encryption schemes
	/// </summary>
	public enum TPMEntityTypeMSB : ushort
	{
		TPM_ET_XOR = 0x00,
		TPM_ET_AES128_CTR = 0x06
	}
	
	/// <summary>
	/// Specifies types of entities
	/// </summary>
	public enum TPMEntityTypeLSB : ushort
	{
		/// <summary>
		/// The entity is a key handle or key
		/// </summary>
		TPM_ET_KEYHANDLE = 0x01,
		
		/// <summary>
		/// The entity is the TPM owner
		/// </summary>
		TPM_ET_OWNER = 0x02,
		
		/// <summary>
		/// The entity is some data
		/// </summary>
		TPM_ET_DATA = 0x03,
		
		/// <summary>
		/// The entity is the SRK
		/// </summary>
		TPM_ET_SRK = 0x04,
		
		/// <summary>
		/// The entity is a key or keyhandle
		/// </summary>
		TPM_ET_KEY = 0x05,
		
		/// <summary>
		/// The entity is the RevokeTrust value
		/// </summary>
		TPM_ET_REVOKE = 0x06,
		
		/// <summary>
		/// The entity is a delegate owner blob
		/// </summary>
		TPM_ET_DEL_OWNER_BLOB = 0x07,
		
		/// <summary>
		/// The entity is a delegate row
		/// </summary>
		TPM_ET_DEL_ROW = 0x08,
		
		/// <summary>
		/// The entity is a delegate key blob
		/// </summary>
		TPM_ET_DEL_KEY_BLOB = 0x09,
		
		/// <summary>
		/// The entity is a counter
		/// </summary>
		TPM_ET_COUNTER = 0x0A,
		
		/// <summary>
		/// The entity is a NV index
		/// </summary>
		TPM_ET_NV = 0x0B,
		
		/// <summary>
		/// The entity is the operator
		/// </summary>
		TPM_ET_OPERATOR = 0x0C
		
	}
	
	/// <summary>
	/// Specifies all available predefined Key Handles
	/// </summary>
	public enum TPMKeyHandles : uint
	{
		/// <summary>
		/// The handle points to the SRK
		/// </summary>
		TPM_KH_SRK = 0x40000000,
		
		/// <summary>
		/// The handle points to the TPM owner
		/// </summary>
		TPM_KH_OWNER = 0x40000001,
		
		/// <summary>
		/// The handle points to the RevokeTrust value
		/// </summary>
		TPM_KH_REVOKE = 0x40000002,
		
		/// <summary>
		/// The handle points to the TPM_EstablishTransport static authorization
		/// </summary>
		TPM_KH_TRANSPORT = 0x40000003,
		
		/// <summary>
		/// The handle points to the operator auth
		/// </summary>
		TPM_KH_OPERATOR = 0x40000004,
		
		/// <summary>
		/// The handle points to the delegation administration auth
		/// </summary>
		TPM_KH_ADMIN = 0x40000005,
		
		/// <summary>
		/// The handle points to ti the PUBEK
		/// </summary>
		TPM_KH_EK = 0x40000006
	}
	
	/// <summary>
	/// This structure specifies the type of payload in various messages.
	/// </summary>
	public enum TPMPayloadType : byte
	{
		/// <summary>
		/// The entity is an asymmetric key
		/// </summary>
		TPM_PT_ASYM = 0x01,
		
		/// <summary>
		/// The entity is bound data
		/// </summary>
		TPM_PT_BIND = 0x02,
		
		/// <summary>
		/// The entity is a migration blob
		/// </summary>
		TPM_PT_MIGRATE = 0x03,
		
		/// <summary>
		/// The entity is a maintenance blob
		/// </summary>
		TPM_PT_MAINT = 0x04,
		
		/// <summary>
		/// The entity is sealed data
		/// </summary>
		TPM_PT_SEAL = 0x05,
		
		/// <summary>
		/// The entity is a restricted-migration asymmetric key
		/// </summary>
		TPM_PT_MIGRATE_RESTRICTED = 0x06,
		
		/// <summary>
		/// The entity is a external migratable key
		/// </summary>
		TPM_PT_MIGRATE_EXTERNAL = 0x07,
		
		/// <summary>
		/// The entity is a CMK migratable blo
		/// </summary>
		TPM_PT_CMK_MIGRATE = 0x08
	}
}
