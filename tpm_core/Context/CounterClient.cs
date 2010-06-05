// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils.Hash;

namespace Iaik.Tc.TPM.Context
{

	public class CounterClient
	{
		/// <summary>
		/// The associated session
		/// </summary>
		private TPMSession _tpmSession;

		
		public CounterClient (TPMSession tpmSession)
		{
			_tpmSession = tpmSession;
		}
		
		/// <summary>
		/// Creates a new counter if possible.
		/// Creating a counter requires the owner password and also the secret_counter secret
		/// </summary>
		/// <param name="label">4 bytes to label the counter</param>
		/// <returns></returns>
		public CounterContext CreateCounter(byte[] label)
		{
			if(label.Length != 4)
				throw new ArgumentException("label needs to be of size 4");
				
			ProtectedPasswordStorage counterSecret = _tpmSession.RequestSecret(new HMACKeyInfo(HMACKeyInfo.HMACKeyType.CounterSecret, new Parameters()));
			
			if(counterSecret.Hashed == false)
				counterSecret.Hash();
				
			counterSecret.DecryptHash();
			
			Parameters createCounterParams = new Parameters();
			createCounterParams.AddPrimitiveType("secret", counterSecret.HashValue);
			createCounterParams.AddPrimitiveType("label", label);
			
			return new CounterContext(_tpmSession,
				_tpmSession.DoTPMCommandRequest(new TPMCommandRequest(TPMCommandNames.TPM_CMD_CreateCounter, createCounterParams))
					.Parameters.GetValueOf<uint>("counter_id")
				);
		}
	}


	/// <summary>
	/// Created by the TPMSession to perform actions on a selected counter
	/// </summary>
	public class CounterContext
	{
		/// <summary>
		/// The associated session
		/// </summary>
		private TPMSession _tpmSession;

		/// <summary>
		/// The id of the counter
		/// </summary>
		private uint _counterId;
		
		public uint CounterId
		{
			get { return _counterId;}
		}
		
		public CounterContext (TPMSession tpmSession, uint counterId)
		{
			_tpmSession = tpmSession;
			_counterId = counterId;
		}
		
		
	}
}
