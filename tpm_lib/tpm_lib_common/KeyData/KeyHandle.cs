// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Library.Common.Handles;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.KeyData
{


	public class KeyHandle : AutoStreamSerializable, ITPMHandle
	{
		public const string KEY_SRK = "srk";
	
		#region ITPMHandle implementation
		
		[SerializeMe(0)]
		private byte[] _contextBlob = null;
		
		public byte[] ContextBlob 
		{
			get { return _contextBlob; }
			set { _contextBlob = value; } 
		}
		
		[SerializeMe(1)]
		private uint _handle;
		
		public uint Handle 
		{
			get { return _handle; }
			set { _handle = value; }
		}
		
		
		public TPMResourceType ResourceType 
		{
			get { return TPMResourceType.TPM_RT_KEY; }
		}
		
		#endregion
		
		[SerializeMe(2)]
		private string _identifier;
		
		public string Identifier
		{
			get{ return _identifier; }
		}
		

		public KeyHandle(Stream src)
		{
			Read(src);
		}
		
		public KeyHandle (string identifier, uint keyHandle)
		{
			_identifier = identifier;
			_handle = keyHandle;
		}
	}
}
