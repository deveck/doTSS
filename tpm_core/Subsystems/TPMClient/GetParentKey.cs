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
	/// Is sent from the server to the client to request the parent key of the specified key
	/// </summary>
	public class GetParentKeyRequest : TypedClientSubsystemRequest<GetParentKeyResponse>
	{
	
		#region Subsystem specific properties
		public override string Subsystem 
		{
			get { return SubsystemConstants.SUBSYSTEM_TPMCLIENT; }
		}
		
		public override ushort RequestIdentifier 
		{
			get { return (ushort)TPMClientSubsystem.TPMClientRequestEnum.GetParentKey; }
		}
		#endregion

		
		private string _identifier = null;
		
		/// <summary>
		/// Gets information about the secret used to generate the hmac
		/// </summary>
		public string Identifier
		{
			get { return _identifier; }
			set { _identifier = value;}
		}
				
		
		public GetParentKeyRequest(EndpointContext ctx)
			:base(ctx)
		{
		}
		
		
		public override void Read (Stream src)
		{
			base.Read (src);
			
			_identifier = StreamHelper.ReadString(src);
		}

		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			StreamHelper.WriteString(_identifier, sink);			
		}

	}
	
	public class GetParentKeyResponse : TPMClientSubsystemResponseBase
	{
		
		private string _parentIdentifier;
		
		
		/// <summary>
		/// The generated hmac authdata
		/// </summary>
		public string ParentIdentifier
		{
			get { return _parentIdentifier;}
			set{ _parentIdentifier = value;}
		}
		
		public GetParentKeyResponse (SubsystemRequest request, EndpointContext ctx) 
			: base(request, ctx)
		{
		}

		public override void Read (Stream src)
		{
			base.Read (src);
			
			_parentIdentifier = StreamHelper.ReadString(src);
		}

		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			StreamHelper.WriteString(_parentIdentifier, sink);
		}

	}
}