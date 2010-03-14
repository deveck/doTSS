// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Context;
using Iaik.Tc.Tpm.Configuration;
using Iaik.Tc.Tpm.lowlevel;
using Iaik.Tc.Tpm.lowlevel.backends.linux;
using Iaik.Tc.Tpm.library.commands;

namespace Iaik.Tc.Tpm.Subsystems.Tpm
{


	public class TpmSubsystem : BaseSubsystem<TpmSubsystem.TpmRequestEnum>
	{
		public override string SubsystemIdentifier 
		{
			get { return SubsystemConstants.SUBSYSTEM_TPM; }
		}		
		

		public enum TpmRequestEnum : ushort
		{
			/// <summary>
			/// Requests a tpm operation 
			/// </summary>
			TpmRequest = 0x0001,
		}		
		
		public TpmSubsystem (EndpointContext context, IConnectionsConfiguration config)
            : base(context, config)
		{
			_requestExecutionInfos.Add (TpmRequestEnum.TpmRequest,
				BuildRequestExecutionInfo<TpmSubsystem, TpmRequest, TpmResponse> (HandleTpmRequest));
		}
		
		public override void HandlePacket (Iaik.Connection.Packets.DataPacket packet)
		{
			base.HandlePacket (packet);
		}

		
		private void HandleTpmRequest (TpmSubsystem subsystem, RequestContext<TpmRequest, TpmResponse> requestContext)
		{
			//TODO: Check if the current connection has an associated user (already authenticated)
			//TODO: Do some permission checking here!
			
			//Just for testing
			TPMProvider tpmDevice = new TddlTpm ();
			tpmDevice.Open ();
			
			TpmCommand cmd = TpmCommandFactory.Create (requestContext.Request.CommandRequest);
			cmd.Init (requestContext.Request.CommandRequest.Parameters, tpmDevice);
			cmd.Process();
			tpmDevice.Close ();
		}
	}
}
