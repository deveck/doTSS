// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Library.PCRDataCore;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Utils.Nonce;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;
using Iaik.Tc.TPM.Library.Common.PCRData;
using Iaik.Tc.TPM.Library.Hash;

namespace Iaik.Tc.TPM.Library.Commands.Integrity
{

	/// <summary>
	/// Quotes the specified PCR values
	/// </summary>
	[TPMCommands(TPMCommandNames.TPM_CMD_Quote)]
	public class TPM_Quote : TPMCommandAuthorizable
	{

		/// <summary>
		/// The outgoing digest for this command
		/// </summary>
		private byte[] _digest = null;	
	
		/// <summary>
		/// Digest of the tpm response
		/// </summary>
		private byte[] _responseDigest = null;
	
		/// <summary>
		/// contains the PCRs to quote
		/// </summary>
		private TPMPCRSelectionCore _pcrSelection = null;
		
		/// <summary>
		/// Nonce included in the quote request, against replay attacks
		/// </summary>
		private byte[] _nonce = null;
		
		public override byte[] Digest 
		{
			get 
			{
				if(_digest == null)
				{

					_digest = new HashProvider().Hash(
						new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_Quote),
						new HashByteDataProvider(_nonce),
						new HashTPMBlobWritableDataProvider(_pcrSelection));
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
					      new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_Quote),
					      //3S
					      new HashStreamDataProvider(_responseBlob, offset + 4, _responseBlob.Length - offset - 4 - authHandleSize, false));
				}
				
				return _responseDigest;
			}
		}


		public override void Init (Parameters param, TPMProvider tpmProvider, TPMWrapper tpmWrapper)
		{
			base.Init (param, tpmProvider, tpmWrapper);
			
			
			_digest = null;
			_responseDigest = null;
			_nonce = NonceGenerator.GenerateByteNonce(20);
			object myType = param.GetValueOf<object>("targetPCR");
			_pcrSelection = param.GetValueOf<TPMPCRSelectionCore>("targetPCR");
				
		}


		protected override TPMCommandResponse InternalProcess ()
		{
			
			TPMBlob requestBlob = new TPMBlob();
			requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_AUTH1_COMMAND, TPMOrdinals.TPM_ORD_Quote);
			
			//key handle gets inserted later, it may be not available now
			requestBlob.WriteUInt32(0);
			requestBlob.Write(_nonce, 0, 20);
			_pcrSelection.WriteToTpmBlob(requestBlob);
						
			AuthorizeMe(requestBlob);
			
			using(_keyManager.AcquireLock())
			{
				requestBlob.SkipHeader();
				requestBlob.WriteUInt32(_keyManager.IdentifierToHandle(_params.GetValueOf<string>("key")).Handle);					
				_responseBlob = TransmitMe(requestBlob);
			}
			
			CheckResponseAuthInfo();
		
			_responseBlob.SkipHeader();
			
						
			Parameters responseParams = new Parameters();
			responseParams.AddValue("pcrData", TPMPCRCompositeCore.CreateFromTPMBlob(_responseBlob)); 
			
			uint sigSize = _responseBlob.ReadUInt32();
			responseParams.AddPrimitiveType("sig", _responseBlob.ReadBytes((int)sigSize));
			
			
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_Quote, responseParams);
		}
		
		
		public override HMACKeyInfo GetKeyInfo (AuthSessionNum authSessionNum)
		{
			if(authSessionNum != AuthSessionNum.Auth1)
				return null;
		
		
			string keyIdentifier = _params.GetValueOf<string>("key");
			
			Parameters parameters = new Parameters();
			parameters.AddPrimitiveType("identifier", keyIdentifier);
			
			return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyUsageSecret, parameters);
		}


	
		public override bool SupportsAuthType (AuthHandle.AuthType authType)
		{
			return authType == AuthHandle.AuthType.OIAP;
		}


		public override string GetHandle (AuthSessionNum authSessionNum)
		{
			if(authSessionNum == AuthSessionNum.Auth1)
				return _params.GetValueOf<string>("key");
			
			return null;
		}

	}
}
