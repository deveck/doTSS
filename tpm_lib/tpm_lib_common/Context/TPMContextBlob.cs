// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.Context
{

	/// <summary>
	/// Wrappes a Saved context
	/// </summary>
	[TypedStreamSerializable("TPMContextBlob")]
	public class TPMContextBlob : AutoStreamSerializable
	{
		[SerializeMe(0)]
		protected TPMStructureTag _tag = TPMStructureTag.TPM_TAG_CONTEXT_BLOB;
		
		public TPMStructureTag Tag
		{
			get{ return _tag; }
		}
		
		[SerializeMe(1)]
		protected TPMResourceType _resourceType;
		
		public TPMResourceType ResourceType
		{
			get{ return _resourceType;}
			set{_resourceType = value;}
		}
		
		[SerializeMe(2)]
		protected uint _handle;
		
		public uint Handle
		{
			get{ return _handle ;}
			set{ _handle = value;}
		}

		[SerializeMe(3)]
		protected byte[] _label;
		
		public byte[] Label
		{
			get{ return _label; }
			set{ _label = value;}
		}
		
		[SerializeMe(4)]
		protected uint _contextCount;
		
		public uint ContextCount
		{
			get{ return _contextCount; }
			set{ _contextCount = value;}
		}
		
		[SerializeMe(5)]
		protected byte[] _integrityDigest;
		
		public byte[] IntegrityDigest
		{
			get{ return _integrityDigest; }
			set{ _integrityDigest = value;}
		}
		
		[SerializeMe(6)]
		protected byte[] _additionalData;
		
		public byte[] AdditionalData
		{
			get{ return _additionalData ;}
			set{_additionalData = value;}
		}
		
		[SerializeMe(7)]
		protected byte[] _sensitiveData;
		
		public byte[] SensitiveData
		{
			get{ return _sensitiveData; }
			set{ _sensitiveData = value;}
		}
		
		public TPMContextBlob ()
		{
		}
		
		public TPMContextBlob(Stream src)
		{
			Read(src);
		}
	}
}
