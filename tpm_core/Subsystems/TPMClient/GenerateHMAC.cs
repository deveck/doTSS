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

namespace Iaik.Tc.TPM.Subsystems.TPMClient
{

	/// <summary>
	/// Is sent from the server to the client to request HMAC calculation
	/// </summary>
	public class GenerateHMACRequest : TypedClientSubsystemRequest<GenerateHMACResponse>
	{
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
		
		public HMACKeyInfo KeyInfo
		{
			get { return _keyInfo; }
			set { _keyInfo = value;}
		}
		
		
		public GenerateHMACRequest (EndpointContext ctx)
			: base(ctx)
		{
			
		}
		
		public override void Read (Stream src)
		{
			base.Read (src);
			
			_keyInfo = new HMACKeyInfo (src);
		}

		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			_keyInfo = new HMACKeyInfo (sink);	
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
