
using System;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.Handles.Authorization
{

	/// <summary>
	/// Specifies informations about the key to use for a hmac operation
	/// </summary>
	public class HMACKeyInfo : AutoStreamSerializable
	{
		public const string PARAM_KEY_IDENTIFIER = "key_id";
		
		/// <summary>
		/// Defines the Type of the hmac key to use.
		/// Some types require additional information (e.g. KeyUsageSecret requires a key identifier)
		/// </summary>
		public enum HMACKeyType
		{
			/// <summary>
			/// The owner secret is used as hmac key
			/// </summary>
			OwnerSecret,
			
			/// <summary>
			/// The SRK secret is used as hmac key
			/// </summary>
			SrkSecret,
			
			/// <summary>
			/// The secret of the specified key is used as hmac key
			/// </summary>
			KeyUsageSecret
		}
		
		[SerializeMe(0)]
		private HMACKeyType _keyType;
		
		/// <summary>
		/// Returns the hmac key type to use for hmac generation
		/// </summary>		
		public HMACKeyType KeyType
		{
			get { return _keyType; }
		}
		
		[SerializeMe(1)]
		private Parameters _parameters;
		
		/// <summary>
		/// May define some parameters needed by the specified HMACKeyType
		/// </summary>
		public Parameters Parameters
		{
			get { return _parameters; }
		}
		
		public HMACKeyInfo (HMACKeyType keyType, Parameters parameters)
		{
			_keyType = KeyType;
			_parameters = parameters;
		}
		
		public HMACKeyInfo (Stream src)
		{
			Read(src);
		}
	}
}
