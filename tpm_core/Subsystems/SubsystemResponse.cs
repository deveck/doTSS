// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Context;
using Iaik.Tc.Tpm.Connection.Packets;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.Tpm.Subsystems
{

	/// <summary>
	/// Baseclass for all subsystem responses
	/// </summary>
	public abstract class SubsystemResponse : SubsystemRequest
	{
		/// <summary>
		/// The request this response belongs to
		/// </summary>
		protected SubsystemRequest _request;
		
		public SubsystemResponse (SubsystemRequest request, EndpointContext ctx)
			:base(ctx)
		{
			_request = request;
		}
		
		/// <summary>
		/// No responses to responses
		/// </summary>
		public override Type ResponseType 
		{
			get { return null; }
		}
		
		public override ushort RequestIdentifier 
		{
			get { return _request.RequestIdentifier; }
		}
		
		public override string Subsystem 
		{
			get { return _request.Subsystem; }
		}
		
		public override DataPacket ConvertToDataPacket ()
		{			
			using(MemoryStream sink = new MemoryStream())
			{
				StreamHelper.WriteUInt16(RequestIdentifier, sink);
				Write(sink);
				
				sink.Flush();
				DataPacket packet = new DataPacket(true, Subsystem, sink.ToArray());
				return packet;
			}
			
		}



 
	}
}
