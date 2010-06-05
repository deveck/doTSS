// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Context
{


	public class RNG : Random
	{
		private TPMSession _tpmSession;
		
		public RNG(TPMSession tpmSession)
		{
			_tpmSession = tpmSession;
		}
		
		protected override double Sample ()
		{
			Parameters getRandomParameters = new Parameters();
			getRandomParameters.AddPrimitiveType("bytes_requested", (uint)4);
			byte[] randomBytes = _tpmSession.DoTPMCommandRequest(new TPMCommandRequest(TPMCommandNames.TPM_CMD_GetRandom, getRandomParameters))
				.Parameters.GetValueOf<byte[]>("data");
				
			
			byte[] realData;
			if(randomBytes.Length <4)
			{
				Console.WriteLine("Requested 4 received {0}", randomBytes.Length);
				realData = new byte[4];
				Array.Copy(randomBytes, 0, realData, 0, randomBytes.Length);
			}
			else
				realData = randomBytes;
				
			UInt32 randomVal = BitConverter.ToUInt32(realData, 0);
			
			return (double)randomVal/(double)UInt32.MaxValue;
		}


	}
}
