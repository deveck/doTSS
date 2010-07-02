// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Handles;
using Iaik.Tc.TPM.Library.Common.Context;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.ContextCore
{

	[TypedStreamSerializable("ContextHandle")]
	public class ContextHandle : AutoStreamSerializable, ITPMHandle
	{
		[SerializeMe(0)]
		private byte[] _rawContextBlob;
		
		[SerializeMe(1)]
		private TPMContextBlob _contextBlob;
		
		public ContextHandle(byte[] rawContextHandle)
		{
			_rawContextBlob = rawContextHandle;
			_contextBlob = TPMContextBlobCore.CreateFromBytes(_rawContextBlob);
		}
		
		public ContextHandle(Stream src)
		{
			Read(src);
		}
		
		#region ITPMHandle implementation
		public byte[] ContextBlob 
		{
			get {return _rawContextBlob; }
		}
		
		
		public bool ForceHandle 
		{
			get { return true; }
		}
		
		
		public uint Handle 
		{
			get { return _contextBlob.ContextCount; }
			set { _contextBlob.ContextCount = value; }
		}
		
		
		public TPMResourceType ResourceType 
		{
			get { return TPMResourceType.TPM_RT_CONTEXT; }
		}
		
		#endregion
	}
}
