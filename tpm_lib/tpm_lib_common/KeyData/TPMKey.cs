
using System;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.KeyData
{

	[TypedStreamSerializable("TPM_Key")]
	public class TPMKey : AutoStreamSerializable, ITypedParameter
	{
		
	
	
		[SerializeMe(0)]
		protected CapabilityData.TPMVersion _version;
		
		public CapabilityData.TPMVersion Version
		{
			get { return _version;}
		}
		
		[SerializeMe(1)]
		protected TPMKeyUsage _keyUsage;
		
		public TPMKeyUsage KeyUsage
		{
			get { return _keyUsage;}
		}
	
		[SerializeMe(2)]
		protected TPMKeyFlags _keyFlags;
		
		public TPMKeyFlags KeyFlags
		{
			get { return _keyFlags;}
		}
		
		[SerializeMe(3)]
		protected TPMAuthDataUsage _authDataUsage;
		
		public TPMAuthDataUsage AuthDataUsage
		{
			get { return _authDataUsage;}
		}
		
		[SerializeMe(4)]
		protected TPMKeyParams _algorithmParams;
		
		public TPMKeyParams AlgorithmParams
		{
			get { return _algorithmParams;}
		}
		
		//TODO: Include PCRInfo
		
		[SerializeMe(6)]
		protected TPMStorePubkey _pubKey;
		
		public TPMStorePubkey PubKey
		{
			get { return _pubKey;}
		}
		
		[SerializeMe(7)]
		protected byte[] _encData;

		public byte[] EncData
		{
			get { return _encData;}
		}
		
		
		protected TPMKey ()
		{
		}
		
		public TPMKey (Stream src)
		{
			Read (src);
		}
	}
}
