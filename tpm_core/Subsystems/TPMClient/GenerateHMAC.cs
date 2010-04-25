// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common;
using System.IO;
using Iaik.Utils;
using Iaik.Utils.Hash;

namespace Iaik.Tc.TPM.Subsystems.TPMClient
{

	/// <summary>
	/// Is sent from the server to the client to request HMAC calculation
	/// </summary>
	public class GenerateHMACRequest : TypedClientSubsystemRequest<GenerateHMACResponse>
	{
		public static GenerateHMACRequest CreateGenerateHMACRequest(EndpointContext ctx, params HashDataProvider[] dps)
		{
			 GenerateHMACRequest request = new GenerateHMACRequest(ctx);
			request._hmacDataProviders = dps;
			return request;
		}
		
		
		#region Subsystem specific properties
		public override string Subsystem 
		{
			get { return SubsystemConstants.SUBSYSTEM_TPMCLIENT; }
		}
		public override ushort RequestIdentifier 
		{
			get { return (ushort)TPMClientSubsystem.TPMClientRequestEnum.GenerateHMAC; }
		}
		#endregion

		
		private HMACKeyInfo _keyInfo = null;
		
		/// <summary>
		/// Gets information about the secret used to generate the hmac
		/// </summary>
		public HMACKeyInfo KeyInfo
		{
			get { return _keyInfo; }
			set { _keyInfo = value;}
		}
		
		private HashDataProvider[] _hmacDataProviders = null;
		
		/// <summary>
		/// Gets the data for hmac generation as they are used
		/// </summary>
		public HashDataProvider[] HMACDataProviders
		{
			get{ return _hmacDataProviders;}
		}
		
//		private AuthHandle _authHandle = null;
//		
//		/// <summary>
//		/// Gets the authorization handle used for the hmac generation process
//		/// (only nonce values are used)
//		/// </summary>
//		public AuthHandle AuthHandle
//		{
//			get{ return _authHandle;}
//			set{ _authHandle = value;}
//		}
//		
//		private byte[] _digest = null;
//		
//		/// <summary>
//		/// The digest generated from the command to execute
//		/// </summary>
//		public byte[] Digest
//		{
//			get{ return _digest; }
//			set{ _digest = value; }
//		}
//		
//		private bool _continueAuthSession = true;
//		
//		public bool ContinueAuthSession
//		{
//			get{ return _continueAuthSession;}
//			set{ _continueAuthSession = value;}
//		}
		
		public GenerateHMACRequest(EndpointContext ctx)
			:base(ctx)
		{
		}
		
		
		public override void Read (Stream src)
		{
			base.Read (src);
			
			_keyInfo = new HMACKeyInfo (src);
			
			int count = StreamHelper.ReadInt32(src);
			_hmacDataProviders = new HashDataProvider[count];
			
			for(int i = 0; i<count; i++)
			{
				_hmacDataProviders[i] = StreamHelper.ReadTypedStreamSerializable<HashDataProvider>(src);
			}
			
//			_authHandle = new AuthHandle(src);
//			_digest = StreamHelper.ReadBytesSafe(src);
//			_continueAuthSession = StreamHelper.ReadBool(src);
		}

		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			_keyInfo.Write(sink);
			
			StreamHelper.WriteInt32(_hmacDataProviders.Length, sink);
			
			foreach(HashDataProvider dp in _hmacDataProviders)
				StreamHelper.WriteTypedStreamSerializable(dp, sink);
			
//			_authHandle.Write(sink);
//			StreamHelper.WriteBytesSafe(_digest, sink);
//			StreamHelper.WriteBool(_continueAuthSession, sink);
			
		}

	}
	
	public class GenerateHMACResponse : TPMClientSubsystemResponseBase
	{
		
		private byte[] _tpmAuthdata = null;
		
		
		/// <summary>
		/// The generated hmac authdata
		/// </summary>
		public byte[] TpmAuthData
		{
			get { return _tpmAuthdata;}
			set{ _tpmAuthdata = value;}
		}
		
		public GenerateHMACResponse (SubsystemRequest request, EndpointContext ctx) 
			: base(request, ctx)
		{
		}

		public override void Read (Stream src)
		{
			base.Read (src);
			
			_tpmAuthdata = StreamHelper.ReadBytesSafe (src);
		}

		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			StreamHelper.WriteBytesSafe (_tpmAuthdata, sink);
		}

	}
}
