// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Tc.TPM.Library.KeyDataCore;
using Iaik.Tc.TPM.Lowlevel;
using System.IO;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Hash;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Library.Commands.StorageFunctions
{

	[TPMCommands(TPMCommandNames.TPM_CMD_LoadKey2)]
	public class TPM_LoadKey2 : TPMCommandAuthorizable
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
		/// Key specification of the key being loaded
		/// </summary>
		private TPMKeyCore _tpmKey = null;
	
		public override byte[] Digest 
		{
			get 
			{
				if(_digest == null)
				{
					_digest = new HashProvider().Hash(
						new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_LoadKey2),
						new HashTPMBlobWritableDataProvider(_tpmKey));
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
					
					_responseDigest = hasher.Hash(
					      //1S
					      new HashStreamDataProvider(_responseBlob, offset, 4, false),
					      //2S
					      new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_LoadKey2));
				}
				
				return _responseDigest;
			}
		}


		public override void Init (Parameters param, TPMProvider tpmProvider)
		{
			base.Init (param, tpmProvider);
			
			_digest = null;
			
			using(MemoryStream src = new MemoryStream(param.GetValueOf<byte[]>("key_blob")))
			{
				_tpmKey =  TPMKeyCore.CreateFromStream(src);
			}
		}


		protected override TPMCommandResponse InternalProcess ()
		{
			TPMBlob requestBlob = new TPMBlob ();
			requestBlob.WriteCmdHeader (TPMCmdTags.TPM_TAG_RQU_AUTH1_COMMAND, TPMOrdinals.TPM_ORD_LoadKey2);			
			
			//If not loaded load now
			if(_params.GetValueOf<uint>("parent_handle") != (uint)TPMKeyHandles.TPM_KH_SRK)
				_keyManager.LoadKey(_params.GetValueOf<string>("parent_identifier"));
						
			//To be inserted later
			requestBlob.WriteUInt32 (0);
			
			_tpmKey.WriteToTpmBlob(requestBlob);
			
			//Blocking authorize, blocks till the user has entered the authorization data
			AuthorizeMe(requestBlob);

			using(_keyManager.AcquireLock())
			{
				uint tpmKeyHandle;
				
				if(_params.GetValueOf<uint>("parent_handle") == (uint)TPMKeyHandles.TPM_KH_SRK)
					tpmKeyHandle = (uint)TPMKeyHandles.TPM_KH_SRK;
				else
					tpmKeyHandle = _keyManager.IdentifierToHandle(_params.GetValueOf<string>("parent_identifier")).Handle;
				
				//Write key handle to the first position after the header
				requestBlob.SkipHeader();
				requestBlob.WriteUInt32(tpmKeyHandle);

				_responseBlob = TransmitMe(requestBlob);
			}

			CheckResponseAuthInfo();			
			
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_LoadKey2, new Parameters());
		}
		
		
		public override HMACKeyInfo GetKeyInfo (AuthSessionNum authSessionNum)
		{
			if(authSessionNum != AuthSessionNum.Auth1)
				return null;
		
		
			if(_params.GetValueOf<uint>("parent_handle") == (uint)TPMKeyHandles.TPM_KH_SRK)
				return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.SrkSecret, new Parameters());
			else
			{
				Parameters hmacKeyInfoParams = new Parameters();
				hmacKeyInfoParams.AddPrimitiveType("identifier", _params.GetValueOf<string>("parent_identifier"));
				return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyUsageSecret, hmacKeyInfoParams);
			}
		}


	
		public override bool SupportsAuthType (AuthHandle.AuthType authType)
		{
			return authType == AuthHandle.AuthType.OIAP;
		}


		
	
	}
}
