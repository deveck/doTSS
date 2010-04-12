
using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.KeyDataCore;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Hash;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Library.Commands.AdminOwnership
{

	[TPMCommands(TPMCommandNames.TPM_CMD_TakeOwnership)]
	public class TPM_TakeOwnership : TPMCommand, IAuthorizableCommand
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
		
		public override void Init (Parameters param, TPMProvider tpmProvider)
		{
			base.Init (param, tpmProvider);
			
			_digest = null;
			
			_tpmKey = TPMKeyCore.Create (
				null,  //use default tpm version
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

		
		public override TPMCommandResponse Process ()
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
			
			AuthorizationInfo authInfo = _commandAuthHelper.AuthorizeCommand (
				this, 
				AuthSessionNum.Auth1, 
				null,
				new HMACKeyInfo (HMACKeyInfo.HMACKeyType.OwnerSecret, null)
				);
			
			WriteAuthorizationInfo (requestBlob, authInfo);
			
			TPMBlob responseBlob = _tpmProvider.TransmitAndCheck (requestBlob);
			return null;
		}
		
		public override void Clear ()
		{
			base.Clear ();
			
			_tpmKey = null;
			_digest = null;
		}


		#region IAuthorizableCommand implementation
		
		/// <summary>
		/// Provides command hmac generation
		/// </summary>
		private ICommandAuthorizationHelper _commandAuthHelper = null;
		
		public byte[] Digest 
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
		
		public AuthHandle GetAuthHandle (AuthSessionNum authSessionNum)
		{
			if (authSessionNum != AuthSessionNum.Auth1)
				throw new ArgumentException ("Command only requires Auth1");
			
			return _params.GetValueOf<AuthHandle> (PARAM_AUTHHANDLE, null);
		}
		
		
		public void SetCommandAuthorizationHelper (ICommandAuthorizationHelper commandAuthHelper)
		{
			_commandAuthHelper = commandAuthHelper;
		}
		
		#endregion

	}
}
