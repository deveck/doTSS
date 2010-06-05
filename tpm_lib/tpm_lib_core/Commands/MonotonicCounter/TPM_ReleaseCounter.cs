// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Utils;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common.Basic;
using Iaik.Tc.TPM.Library.Basic;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;

namespace Iaik.Tc.TPM.Library.Commands.MonotonicCounter
{


	[TPMCommands(TPMCommandNames.TPM_CMD_ReleaseCounter)]
	public class TPM_ReleaseCounter : TPMCommandAuthorizable
	{
		/// <summary>
		/// The outgoing digest for this command
		/// </summary>
		private byte[] _digest = null;
	
	
		/// <summary>
		/// Digest of the tpm response
		/// </summary>
		private byte[] _responseDigest = null;
	
	
		public override byte[] Digest 
		{
			get 
			{
				if(_digest == null)
				{
					_digest = new HashProvider().Hash(
						new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_ReleaseCounter),
						new HashPrimitiveDataProvider(_params.GetValueOf<uint>("counter_id")));
				}
				
				return _digest;
			}
		}

	
		public override byte[] ResponseDigest 
		{
			get 
			{
				if(_responseDigest == null)
				{
					HashProvider hasher = new HashProvider();
					
					int offset = 2+4; //tag + paramsize
					
					int authHandleSize = ResponseAuthHandleInfoCore.ReadAuthHandleInfos(this, _responseBlob).Length *
						ResponseAuthHandleInfoCore.SINGLE_AUTH_HANDLE_SIZE;
						
					_responseDigest = hasher.Hash(
					      //1S
					      new HashStreamDataProvider(_responseBlob, offset, 4, false),
					      //2S
					      new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_ReleaseCounter),
					      //3S
					      new HashStreamDataProvider(_responseBlob, offset + 4, _responseBlob.Length - offset - 4 - authHandleSize, false));
				}
				
				return _responseDigest;
			}
		}

	
	
		protected override TPMCommandResponse InternalProcess ()
		{
						
			using(TPMBlob requestBlob = new TPMBlob())
			{
				requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_AUTH1_COMMAND, TPMOrdinals.TPM_ORD_ReleaseCounter);
				requestBlob.WriteUInt32(_params.GetValueOf<uint>("counter_id"));
				
				_responseBlob = AuthorizeMeAndTransmit(requestBlob);				
			}
			
			_responseParameters = new Parameters();
			
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_CreateCounter, _responseParameters);
		}
		
		public override HMACKeyInfo GetKeyInfo (AuthSessionNum authSessionNum)
		{
			if(authSessionNum == AuthSessionNum.Auth1)
				return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.CounterSecret, new Parameters());
				
			return null;
		}
		
		public override bool SupportsAuthType (AuthHandle.AuthType authType)
		{
			return authType == AuthHandle.AuthType.OIAP;
		}


	}
}
