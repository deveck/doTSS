// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.TPM.Context;
using Iaik.Utils;
using Iaik.Tc.TPM.Configuration;
using Iaik.Tc.TPM.Authentication;
using System.Collections.Generic;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{
	/// <summary>
	/// Requests all available authentication modes for the current connection
	/// from the server
	/// </summary>
	public class TPMRequest : TypedSubsystemRequest<TPMResponse>
	{
		
		
		
		public override string Subsystem 
		{
			get { return SubsystemConstants.SUBSYSTEM_TPM; }
		}

		public override ushort RequestIdentifier
		{
			get	{ return (ushort)TPMSubsystem.TPMRequestEnum.TPMRequest; }
		}
				
		
		/// <summary>
		/// The request with its parameters 
		/// </summary>
		private TPMCommandRequest _commandRequest = null;
		
		public TPMCommandRequest CommandRequest
		{
			get { return _commandRequest; }
			set { _commandRequest = value;}
		}

		public TPMRequest (EndpointContext ctx)
			: base(ctx)
		{
		}
		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			_commandRequest.Write (sink);
		}
 
		
		public override void Read (Stream src)
		{
			base.Read (src);
			
			_commandRequest = new TPMCommandRequest (src);
		}

	}
	
	
	/// <summary>
	/// Response to a <see>TpmRequest</see>
	/// </summary>
	public class TPMResponse : TPMSubsystemResponseBase
	{
		
		public TPMResponse(SubsystemRequest request, EndpointContext ctx) 
			:base(request, ctx)
		{
		}
		
		public override void Read (Stream src)
		{
			base.Read (src);
		}

		public override void Write (Stream sink)
		{
			base.Write (sink);			
		}

		
	}
}
