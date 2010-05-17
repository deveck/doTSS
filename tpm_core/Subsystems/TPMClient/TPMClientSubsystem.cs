// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Configuration;
using Iaik.Tc.TPM.Authentication;
using System.Collections.Generic;
using Iaik.Utils.CommonFactories;
using System.Security.Cryptography;
using Iaik.Utils.Hash;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Subsystems.TPMClient
{

	/// <summary>
	/// Implements the subsystem the server uses to request authorisation data from the client
	/// </summary>
	public class TPMClientSubsystem : BaseClientSubsystem<TPMClientSubsystem.TPMClientRequestEnum>
	{
		public enum TPMClientRequestEnum : ushort
		{
			/// <summary>
			/// Generates the TPM_AUTHDATA with the specified keydata
			/// </summary>
			GenerateHMAC	= 0x0001,
			
			/// <summary>
			///Gets the parent key for the specified key 
			/// </summary>
			GetParentKey,
			
			/// <summary>
			/// Gets the data for the specified key 
			/// </summary>
			GetKeyData,
			
			/// <summary>
			///Checks if the specified key is contained in the key storage 
			/// </summary>
			ContainsKey
		}
		
		public override string SubsystemIdentifier 
		{
			get { return SubsystemConstants.SUBSYSTEM_TPMCLIENT; }
		}
		
		public ClientContext MyClientContext
		{
			get { return (ClientContext)_context;}
		}
		
        public TPMClientSubsystem (ClientContext ctx)
			: base(ctx)
        {
        	if (typeof(ClientContext).IsAssignableFrom (ctx.GetType ()) == false)
        		throw new ArgumentException ("TpmSubsystem requires a ClientContext");
   
			_requestExecutionInfos.Add (TPMClientRequestEnum.GenerateHMAC,
				BuildRequestExecutionInfo<TPMClientSubsystem, GenerateHMACRequest, GenerateHMACResponse> (HandleGenerateHMACRequest));
				
			_requestExecutionInfos.Add(TPMClientRequestEnum.GetParentKey,
				BuildRequestExecutionInfo<TPMClientSubsystem, GetParentKeyRequest, GetParentKeyResponse> (HandleGetParentKeyRequest));
				
			_requestExecutionInfos.Add(TPMClientRequestEnum.GetKeyData,
				BuildRequestExecutionInfo<TPMClientSubsystem, GetKeyDataRequest, GetKeyDataResponse> (HandleGetKeyDataRequest));
				
			_requestExecutionInfos.Add(TPMClientRequestEnum.ContainsKey,
				BuildRequestExecutionInfo<TPMClientSubsystem, ContainsKeyRequest, ContainsKeyResponse> (HandleContainsKeyRequest));
        }



        #region Request handlers
		/// <summary>
		/// Handles a HMAC generation request from the server
		/// </summary>
		/// <param name="subsystem"></param>
		/// <param name="requestContext"></param>
        private void HandleGenerateHMACRequest (TPMClientSubsystem subsystem, RequestContext<GenerateHMACRequest, GenerateHMACResponse> requestContext)
        {
        	TPMSession session = MyClientContext.TPMClient.FindSession (requestContext.Request.TpmSessionIdentifier);
        	
			GenerateHMACResponse response = requestContext.CreateResponse ();
   
			if (session == null)
			{
        		_logger.WarnFormat ("Received HMAC request for tpm session with id #{0}, but this id is not associated with an active session!", 
					requestContext.Request.TpmSessionIdentifier);
    
				response.Succeeded = false;
        		response.SetKnownErrorCode (GenerateHMACResponse.ErrorCodeEnum.TPMSessionNotFound);
        		response.Execute ();
        		return;
			}
			
			_logger.DebugFormat("Requesting password: {0}", requestContext.Request.KeyInfo.KeyType);
			ProtectedPasswordStorage pw = session.RequestSecret(requestContext.Request.KeyInfo);
			if(pw == null)
			{
				response.Succeeded = false;
				response.SetKnownErrorCode(GenerateHMACResponse.ErrorCodeEnum.HMACSecretMissing);
				response.Execute();
				return;
			}

			HMACProvider hmacProvider = new HMACProvider(pw);
			response.Succeeded = true;
        	response.TpmAuthData = hmacProvider.Hash(requestContext.Request.HMACDataProviders);
//			    new HashByteDataProvider(requestContext.Request.Digest),
//			    new HashByteDataProvider(requestContext.Request.AuthHandle.NonceEven),
//			    new HashByteDataProvider(requestContext.Request.AuthHandle.NonceOdd),
//			    new HashPrimitiveDataProvider(requestContext.Request.ContinueAuthSession));
			
        	_logger.Debug("Executing response");
        	response.Execute();
		}
		
		/// <summary>
		/// Looks for the parent key of the specified key
		/// </summary>
		/// <param name="subsystem"></param>
		/// <param name="requestContext"></param>
        private void HandleGetParentKeyRequest (TPMClientSubsystem subsystem, RequestContext<GetParentKeyRequest, GetParentKeyResponse> requestContext)
        {
        	TPMSession session = MyClientContext.TPMClient.FindSession (requestContext.Request.TpmSessionIdentifier);
        	
			GetParentKeyResponse response = requestContext.CreateResponse ();
   
			if (session == null)
			{
        		_logger.WarnFormat ("Received HMAC request for tpm session with id #{0}, but this id is not associated with an active session!", 
					requestContext.Request.TpmSessionIdentifier);
    
				response.Succeeded = false;
        		response.SetKnownErrorCode (TPMClientSubsystemResponseBase.ErrorCodeEnum.TPMSessionNotFound);
        		response.Execute ();
        		return;
			}
			
			if(session.Keystore.ContainsIdentifier(requestContext.Request.Identifier) == false)
			{
				response.SetKnownErrorCode(TPMClientSubsystemResponseBase.ErrorCodeEnum.KeyIdentifierMissing);
				response.Succeeded = false;
				response.Execute();
				return;
			}
			
			KeyValuePair<string, string>? parent = session.Keystore.FindParentKeyByIdentifier(requestContext.Request.Identifier);
			
			if(parent == null || parent.Value.Value == null)
				response.ParentIdentifier = "srk";
			else
				response.ParentIdentifier = parent.Value.Value;			
        	
        	response.Execute();
		}
		
		/// <summary>
		/// Gets the key data (which is needed to load the key)
		/// </summary>
		/// <param name="subsystem"></param>
		/// <param name="requestContext"></param>
        private void HandleGetKeyDataRequest (TPMClientSubsystem subsystem, RequestContext<GetKeyDataRequest, GetKeyDataResponse> requestContext)
        {
        	TPMSession session = MyClientContext.TPMClient.FindSession (requestContext.Request.TpmSessionIdentifier);
        	
			GetKeyDataResponse response = requestContext.CreateResponse ();
   
			if (session == null)
			{
        		_logger.WarnFormat ("Received HMAC request for tpm session with id #{0}, but this id is not associated with an active session!", 
					requestContext.Request.TpmSessionIdentifier);
    
				response.Succeeded = false;
        		response.SetKnownErrorCode (TPMClientSubsystemResponseBase.ErrorCodeEnum.TPMSessionNotFound);
        		response.Execute ();
        		return;
			}
			
			if(session.Keystore.ContainsIdentifier(requestContext.Request.Identifier) == false)
			{
				response.SetKnownErrorCode(TPMClientSubsystemResponseBase.ErrorCodeEnum.KeyIdentifierMissing);
				response.Succeeded = false;
				response.Execute();
				return;
			}
			
			response.KeyData = session.Keystore.GetKeyBlob(requestContext.Request.Identifier);
        	response.Execute();
		}
		
		/// <summary>
		/// Checks if the specified key is contained in the associated key storage
		/// </summary>
		/// <param name="subsystem"></param>
		/// <param name="requestContext"></param>
        private void HandleContainsKeyRequest (TPMClientSubsystem subsystem, RequestContext<ContainsKeyRequest, ContainsKeyResponse> requestContext)
        {
        	TPMSession session = MyClientContext.TPMClient.FindSession (requestContext.Request.TpmSessionIdentifier);
        	
			ContainsKeyResponse response = requestContext.CreateResponse ();
   
			if (session == null)
			{
        		_logger.WarnFormat ("Received contains key request for tpm session with id #{0}, but this id is not associated with an active session!", 
					requestContext.Request.TpmSessionIdentifier);
    
				response.Succeeded = false;
        		response.SetKnownErrorCode (TPMClientSubsystemResponseBase.ErrorCodeEnum.TPMSessionNotFound);
        		response.Execute ();
        		return;
			}
			
			response.Contained =  session.Keystore.ContainsIdentifier(requestContext.Request.Identifier);
        	response.Execute();
		}
		
        #endregion

    }
}

