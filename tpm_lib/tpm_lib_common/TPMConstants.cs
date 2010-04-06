
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
}
