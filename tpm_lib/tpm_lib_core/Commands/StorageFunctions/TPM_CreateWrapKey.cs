// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Tc.TPM.Library.KeyDataCore;
using Iaik.Tc.TPM.Lowlevel;

namespace Iaik.Tc.TPM.Library.Commands.StorageFunctions
{

	[TPMCommands(TPMCommandNames.TPM_CMD_CreateWrapKey)]
	public class TPM_CreateWrapKey : TPMCommandAuthorizable
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
		/// Key specification of the key being created
		/// </summary>
		private TPMKey _tpmKey = null;
	
		public override byte[] Digest 
		{
			get 
			{
				if(_digest == null)
				{
					
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
				}
				
				return _responseDigest;
			}
		}


		public override void Init (Parameters param, TPMProvider tpmProvider)
		{
			base.Init (param, tpmProvider);
			
			_digest = null;
			
			_tpmKey = TPMKeyCore.Create (
			    CapabilityDataCore.TPMVersionCore.CreateVersion11(),
				_params.GetValueOf<TPMKeyUsage>("key_usage"),
				_params.GetValueOf<TPMKeyFlags>("key_flags"),
				TPMAuthDataUsage.TPM_AUTH_ALWAYS,
				TPMKeyParamsCore.Create (
					TPMAlgorithmId.TPM_ALG_RSA, 
					TPMEncScheme.TPM_ES_RSAESOAEP_SHA1_MGF1, 
					TPMSigScheme.TPM_SS_NONE,
					TPMRSAKeyParamsCore.Create (
						_params.GetValueOf<uint>("key_length"), 
						_params.GetValueOf<uint>("num_primes"),
						_params.GetValueOf<byte[]>("exponent"))
					),
				null, //Pubkey, use default (empty) pubkey
				null  //no encoded data
				);
		}


		protected override TPMCommandResponse InternalProcess ()
		{
			// Unencrypted authorization values, they need to be XOR-Encrypted with
			// XOR(auth, SHA-1(OSAP shared secret | session nonce))
			//
			// OSAP_shared_secret = HMAC(key=usage secret of key handle, nonce even osap | nonce odd osap)
			
			byte[] usageAuth = _params.GetValueOf<byte[]> ("usage_auth");
			byte[] migrationAuth = _params.GetValueOf<byte[]> ("migration_auth");
			
			
		
			return null;
		}
		
		
		public override HMACKeyInfo GetKeyInfo (AuthSessionNum authSessionNum)
		{
			if(authSessionNum != AuthSessionNum.Auth1)
				return null;
		
			string parentIdentifier = _params.GetValueOf<string>("parent");
			
			Parameters parameters = new Parameters();
			parameters.AddPrimitiveType("identifier", parentIdentifier);
			
			return new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyUsageSecret, parameters);
			
		}


	
		public override bool SupportsAuthType (AuthHandle.AuthType authType)
		{
			return authType == AuthHandle.AuthType.OSAP;
		}


		
	
	}
}
