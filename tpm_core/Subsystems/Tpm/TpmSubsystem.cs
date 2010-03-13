// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Context;
using Iaik.Tc.Tpm.Configuration;

namespace Iaik.Tc.Tpm.Subsystems.Tpm
{


	public class TpmSubsystem : BaseSubsystem<TpmSubsystem.TpmRequestEnum>
	{
		public override string SubsystemIdentifier 
		{
			get { return SubsystemConstants.SUBSYSTEM_TPM; }
		}		
		

		public enum TpmRequestEnum
		{
			
		}		
		
		public TpmSubsystem(EndpointContext context, IConnectionsConfiguration config)
            : base(context, config)
		{
		}
	}
}
