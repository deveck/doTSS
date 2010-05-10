
using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.KeyDataCore;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Hash;
using Iaik.Tc.TPM.Lowlevel.Data;
using System.IO;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;

namespace Iaik.Tc.TPM.Library.Commands.AdminOwnership
{

	/// <summary>
	/// This command inserts the TPM Ownership value into the TPM.
	/// </summary>
	[TPMCommands(TPMCommandNames.TPM_CMD_TakeOwnership)]
	public class TPM_TakeOwnership : TPMCommandAuthorizable
	{
		/// <summary>
		/// Specifies the authorization handle to use for this command
		/// </summary>
		public const string PARAM_AUTHHANDLE = "authhandle";

		/// <summary>
		/// The owner authdata encrypted with pubek
		/// </summary>
		public const string PARAM_OWNERAUTH = "owner_auth";

		/// <summary>
		/// The srk authdata encrypted with pubek
		/// </summary>
		public const string PARAM_SRKAUTH = "srk_auth";
		
		/// <summary>
		/// Key structure containing details about the SRK
		/// </summary>
		private TPMKeyCore _tpmKey = null;
		
		/// <summary>
		/// the in parameter digest (1HX)
		/// </summary>
		private byte[] _digest = null;
		
		/// <summary>
		/// The out parameter digest (1HX)
		/// </summary>
		private byte[] _responseDigest = null;
		
		
		
		
		public override void Init (Parameters param, TPMProvider tpmProvider)
		{
			base.Init (param, tpmProvider);
			
			_digest = null;
			
			_tpmKey = TPMKeyCore.Create (
			    CapabilityDataCore.TPMVersionCore.CreateVersion11(),
				TPMKeyUsage.TPM_KEY_STORAGE, 
				TPMKeyFlags.None,
				TPMAuthDataUsage.TPM_AUTH_ALWAYS,
				TPMKeyParamsCore.Create (
					TPMAlgorithmId.TPM_ALG_RSA, 
					TPMEncScheme.TPM_ES_RSAESOAEP_SHA1_MGF1, 
					TPMSigScheme.TPM_SS_NONE,
					TPMRSAKeyParamsCore.Create (TPMRSAKeyParamsCore.DEFAULT_KEYLENGTH, TPMRSAKeyParamsCore.DEFAULT_NUMPRIMES, null)
					),
				null, //Pubkey, use default (empty) pubkey
				null  //no encoded data
				);
		}

		
		protected override TPMCommandResponse InternalProcess ()
		{
			byte[] ownerAuth = _params.GetValueOf<byte[]> (PARAM_OWNERAUTH);
			byte[] srkAuth = _params.GetValueOf<byte[]> (PARAM_SRKAUTH);
			
			
			TPMBlob requestBlob = new TPMBlob ();
			requestBlob.WriteCmdHeader (TPMCmdTags.TPM_TAG_RQU_AUTH1_COMMAND, TPMOrdinals.TPM_ORD_TakeOwnership);
			
			requestBlob.WriteUInt16 ((ushort)TPMProtocolId.TPM_PID_OWNER);
			
			requestBlob.WriteUInt32 ((uint)ownerAuth.Length);
			requestBlob.Write (ownerAuth, 0, ownerAuth.Length);
			
			requestBlob.WriteUInt32 ((uint)srkAuth.Length);
			requestBlob.Write (srkAuth, 0, srkAuth.Length);
			
			_tpmKey.WriteToTpmBlob (requestBlob);
			
			_responseBlob = AuthorizeMeAndTransmit(requestBlob);

			CheckResponseAuthInfo();			
			
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_TakeOwnership, new Parameters());
		}
		
		public override void Clear ()
		{
			base.Clear ();
			
			_tpmKey = null;
			_digest = null;
		}


		#region IAuthorizableCommand implementation
		
		public override byte[] Digest 
		{
			get 
			{
				if (_digest == null)
				{
					byte[] ownerAuth = _params.GetValueOf<byte[]> (PARAM_OWNERAUTH);
					byte[] srkAuth = _params.GetValueOf<byte[]> (PARAM_SRKAUTH);
					
					HashProvider hasher = new HashProvider ();
					_digest = hasher.Hash (
						new HashPrimitiveDataProvider (TPMOrdinals.TPM_ORD_TakeOwnership),
						new HashEnumDataProvider (TPMProtocolId.TPM_PID_OWNER),
						new HashPrimitiveDataProvider ((uint)ownerAuth.Length),
						new HashByteDataProvider (ownerAuth),
						new HashPrimitiveDataProvider ((uint)srkAuth.Length),
						new HashByteDataProvider (srkAuth),
						new HashTPMBlobWritableDataProvider (_tpmKey)
						);
						
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
					      new HashPrimitiveDataProvider(TPMOrdinals.TPM_ORD_TakeOwnership),
					      //3S
					      new HashStreamDataProvider(_responseBlob, offset + 4, _responseBlob.Length - offset - 4 - authHandleSize, false));
				}
				
				return _responseDigest;
				
			}
		}
		
		public override bool SupportsAuthType(AuthHandle.AuthType authType)
		{
			//TPM_TakeOwnership only supports OIAP
			return authType == AuthHandle.AuthType.OIAP;
		}
		
		public override HMACKeyInfo GetKeyInfo(AuthSessionNum authSessionNum)
		{
			if (authSessionNum != AuthSessionNum.Auth1)
				return null;
			
			return new HMACKeyInfo (HMACKeyInfo.HMACKeyType.OwnerSecret, null);
		}
		#endregion

	}
}
