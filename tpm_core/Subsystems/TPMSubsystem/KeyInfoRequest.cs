// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Context;
using Iaik.Utils;
using System.IO;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.KeyData;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{


	public class KeyInfoRequest : TypedSubsystemRequest<KeyInfoResponse>
	{
		public override ushort RequestIdentifier {
			get {
				return (ushort)TPMSubsystem.TPMRequestEnum.KeyInfo;
			}
		}
		
		public override string Subsystem {
			get {
				return SubsystemConstants.SUBSYSTEM_TPM;
			}
		}
		

		/// <summary>
		/// Identifies the key
		/// </summary>
		private string _keyIdentifier;

		public string KeyIdentifier
		{
			get{ return _keyIdentifier; }
			set{ _keyIdentifier = value; }
		}

		private int _tpmIdentifier;
		
		/// <summary>
		/// Identifies the TPM this action should take place on.
		/// Acquire this id by selecting the tpm device 
		/// </summary>
		public int TPMIdentifier
		{
			get { return _tpmIdentifier; }
			set { _tpmIdentifier = value;}
		}
		
		public KeyInfoRequest (EndpointContext ctx)
			:base(ctx)
		{
		}
		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			StreamHelper.WriteInt32(_tpmIdentifier, sink);
			StreamHelper.WriteString(_keyIdentifier, sink);
		}
		
		public override void Read (Stream src)
		{
			base.Read (src);
			
			_tpmIdentifier = StreamHelper.ReadInt32(src);
			_keyIdentifier = StreamHelper.ReadString(src);
		}


	}
	
	
	public class KeyInfoResponse  : TPMSubsystemResponseBase
	{
		
		private TPMKey _tpmKey;
		
		/// <summary>
		/// Contains informations about the key
		/// </summary>
		public TPMKey TPMKey
		{
			get{ return _tpmKey; }
			set{ _tpmKey = value;}
		}
		
		
				
								
		public KeyInfoResponse (SubsystemRequest request, EndpointContext ctx)
			: base(request, ctx)
		{
		}
		
		public override void Read (Stream src)
		{
			base.Read (src);
		
			_tpmKey = StreamHelper.ReadTypedStreamSerializable<TPMKey>(src);
		}
		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			StreamHelper.WriteTypedStreamSerializable(_tpmKey, sink);
		}


	}
}
