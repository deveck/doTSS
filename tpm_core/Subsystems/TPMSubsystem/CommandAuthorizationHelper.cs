// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Subsystems.TPMClient;
using System.Collections.Generic;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.KeyData;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{


	public class CommandAuthorizationHelper : ICommandAuthorizationHelper
	{
		
		/// <summary>
		/// The EndpointContext this CommandAuthorizationHelper belongs to
		/// </summary>
		private ServerContext _ctx;
		
		/// <summary>
		/// The TPM session identifier this CommandAuthorizationHelper belongs to.
		/// </summary>
		private int _tpmSessionIdentifier;
		
		private TPMContext _tpmContext;
		
		public CommandAuthorizationHelper (ServerContext ctx, int tpmSessionIdentifier, TPMContext tpmContext)
		{
			_ctx = ctx;
			_tpmSessionIdentifier = tpmSessionIdentifier;
			_tpmContext = tpmContext;
		}
		
		#region ICommandAuthorizationHelper implementation
		
		/// <summary>
		/// Assures that the shared secret for the specified authorization handle has been
		/// calculated, if not it gets calculated. If no OSAP session exists, create it
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="sessionNum"></param>
		/// <returns></returns>
		public AuthHandle AssureOSAPSharedSecret(IAuthorizableCommand cmd, AuthSessionNum authSessionNum)
		{
		
			lock(_tpmContext.AuthHandleManager)
			{
				//Must not be called for OSAP at the moment because OSAP session are not cached
				_tpmContext.AuthHandleManager.ReserveAuthHandleSlots(cmd);
			}
			
			HMACKeyInfo keyInfo = cmd.GetKeyInfo(authSessionNum);
			
			if(keyInfo == null)
				return null;

			AuthHandle authHandle; 
				
			lock(_tpmContext.AuthHandleManager)
			{					
				authHandle = _tpmContext.AuthHandleManager.GetAuthHandle(cmd, authSessionNum);
			}
	
			// If shared secret has not yet been generated, do it
			if(authHandle.SharedSecret == null)
			{
				GenerateHMACRequest request = GenerateHMACRequest.CreateGenerateHMACRequest(
					_ctx,
					new HashByteDataProvider(authHandle.NonceEvenOSAP),
					new HashByteDataProvider(authHandle.NonceOddOSAP)
					);
					
				request.TpmSessionIdentifier = _tpmSessionIdentifier;
				
				Parameters paramsSharedSecret = new Parameters();
				
				if(cmd.GetHandle(authSessionNum) == KeyHandle.KEY_SRK)
					request.KeyInfo = new HMACKeyInfo(HMACKeyInfo.HMACKeyType.SrkSecret, new Parameters());
				else
				{
					paramsSharedSecret.AddPrimitiveType("identifier", cmd.GetHandle(authSessionNum));
					request.KeyInfo = new HMACKeyInfo(HMACKeyInfo.HMACKeyType.KeyUsageSecret, paramsSharedSecret);
				}
				
				GenerateHMACResponse response = request.TypedExecute ();
				response.AssertResponse();
				
				authHandle.SharedSecret = response.TpmAuthData;
			}
			
			return authHandle;
		}
		
		
		/// <summary>
		/// Authorizes the command and returns the necessary authorization info
		/// Blocks till the user has entered the credentials
		/// </summary>
		/// <param name="cmd">Command to authorize</param>
		/// <returns></returns>
		public AuthorizationInfo[] AuthorizeCommand (IAuthorizableCommand cmd)
		{
			List<AuthorizationInfo> authorizationInfos = new List<AuthorizationInfo>();
			
			lock(_tpmContext.AuthHandleManager)
			{
				_tpmContext.AuthHandleManager.ReserveAuthHandleSlots(cmd);
			}
			
			foreach(AuthSessionNum authSessionNum in new AuthSessionNum[]{AuthSessionNum.Auth1, AuthSessionNum.Auth2})
			{
				HMACKeyInfo keyInfo = cmd.GetKeyInfo(authSessionNum);
				
				if(keyInfo == null)
					continue;

				AuthHandle authHandle; 
					
				lock(_tpmContext.AuthHandleManager)
				{					
					authHandle = _tpmContext.AuthHandleManager.GetAuthHandle(cmd, authSessionNum);
				}
				
				//Generates the new nonceOdd before the client generates the auth data
				authHandle.NewNonceOdd();
				
				if(authHandle.HandleAuthType == AuthHandle.AuthType.OIAP)
				{
					GenerateHMACRequest request = GenerateHMACRequest.CreateGenerateHMACRequest
						(_ctx,
						 new HashByteDataProvider(cmd.Digest),
						 new HashByteDataProvider(authHandle.NonceEven),
						 new HashByteDataProvider(authHandle.NonceOdd),
						 new HashPrimitiveDataProvider(true)
						 );					 
					                                                       
					                                                      
					request.TpmSessionIdentifier = _tpmSessionIdentifier;
					request.KeyInfo = keyInfo;

					
					GenerateHMACResponse response = request.TypedExecute ();
					response.AssertResponse();
					
					authorizationInfos.Add(new AuthorizationInfo(authHandle, true, response.TpmAuthData));
				}
				else if(authHandle.HandleAuthType == AuthHandle.AuthType.OSAP)
				{
					AssureOSAPSharedSecret(cmd, authSessionNum);					
					
					GenerateHMACRequest request = GenerateHMACRequest.CreateGenerateHMACRequest
						(_ctx,
						 new HashByteDataProvider(cmd.Digest),
						 new HashByteDataProvider(authHandle.NonceEven),
						 new HashByteDataProvider(authHandle.NonceOdd),
						 new HashPrimitiveDataProvider(false)
						 );					 
					                                                       
					                                                      
					request.TpmSessionIdentifier = _tpmSessionIdentifier;
					request.KeyInfo = keyInfo;
		
					
					GenerateHMACResponse response = request.TypedExecute ();
					response.AssertResponse();
					
					authorizationInfos.Add(new AuthorizationInfo(authHandle, false, response.TpmAuthData));
					
				}
			}
			
			return authorizationInfos.ToArray();			
		}
		
		
		public AuthorizationInfo[] GenerateResponseAuthData (IAuthorizableCommand cmd)
		{
			List<AuthorizationInfo> authorizationInfos = new List<AuthorizationInfo>();
			
			List<ResponseAuthHandleInfo> responseAuthHandleInfos = new List<ResponseAuthHandleInfo>(cmd.ResponseAuthHandleInfos);
			responseAuthHandleInfos.Reverse();
			
			List<AuthorizationInfo> localAuthorizationInfos = new List<AuthorizationInfo>(cmd.AuthorizationInfos);
			localAuthorizationInfos.Reverse();
				
			Queue<ResponseAuthHandleInfo> responseAuthHandles = new Queue<ResponseAuthHandleInfo>(responseAuthHandleInfos);
			Queue<AuthorizationInfo> authorizationInfoQueue = new Queue<AuthorizationInfo>(localAuthorizationInfos);
			
			foreach(AuthSessionNum authSessionNum in new AuthSessionNum[]{AuthSessionNum.Auth1, AuthSessionNum.Auth2})
			{
				HMACKeyInfo keyInfo = cmd.GetKeyInfo(authSessionNum);
				
				if(keyInfo == null)
					continue;
				
				ResponseAuthHandleInfo currentResponseAuthHandleInfo = responseAuthHandles.Dequeue();
				AuthorizationInfo currentAuthorizationInfo = authorizationInfoQueue.Dequeue();
				
				GenerateHMACRequest request = GenerateHMACRequest.CreateGenerateHMACRequest
					(_ctx,
					 new HashByteDataProvider(cmd.ResponseDigest),
					 new HashByteDataProvider(currentResponseAuthHandleInfo.NonceEven),
					 new HashByteDataProvider(currentAuthorizationInfo.Handle.NonceOdd),
					 new HashPrimitiveDataProvider(currentResponseAuthHandleInfo.ContinueAuthSession)
					 );					 
				                                                       
				                                                      
				request.TpmSessionIdentifier = _tpmSessionIdentifier;
				request.KeyInfo = keyInfo;
//				request.AuthHandle = authHandle;
//				request.Digest = cmd.Digest;
//				request.ContinueAuthSession = true;
//				
				
				GenerateHMACResponse response = request.TypedExecute ();
				response.AssertResponse();
				
				authorizationInfos.Add(new AuthorizationInfo(null, currentResponseAuthHandleInfo.ContinueAuthSession, response.TpmAuthData));
			}
			
			return authorizationInfos.ToArray();	
		}
		
		/// Extracts the AuthHandles and loads them
		/// </summary>
		/// <param name="authInfos">
		/// A <see cref="AuthorizationInfo[]"/>
		/// </param>
		public void LoadAuthorizationHandles(params AuthorizationInfo[] authInfos)
		{
			LoadAuthorizationHandles(AuthInfoToAuthHandle(authInfos));
		}
		
		/// <summary>
		/// Loads the auth handles into the tpm
		/// </summary>
		/// <param name="authHandles">
		/// A <see cref="AuthHandle[]"/>
		/// </param>
		public void LoadAuthorizationHandles(params AuthHandle[] authHandles)
		{
			foreach(AuthHandle authHandle in authHandles)
				_tpmContext.AuthHandleManager.LoadAuthHandle(authHandle);
		}
		
		public void DestroyAuthorizationHandles (IAuthorizableCommand cmd)
		{
			_tpmContext.AuthHandleManager.DestroyAuthHandles(cmd);
		}
		
		
		public void ReleaseAuthorizationHandles (IAuthorizableCommand cmd)
		{
			_tpmContext.AuthHandleManager.ReleaseAuthHandles(cmd);
		}
		
		public void DestroyAuthorizationHandle (AuthHandle handle)
		{
			_tpmContext.AuthHandleManager.DestroyAuthHandles(handle);
		}
		
		
		public void RemoveAuthorizationHandle (IAuthorizableCommand cmd)
		{
			_tpmContext.AuthHandleManager.RemoveAuthHandles(cmd);
		}
		
		
		#endregion
		
		private AuthHandle[] AuthInfoToAuthHandle(AuthorizationInfo[] authInfos)
		{
			List<AuthHandle> authHandles = new List<AuthHandle>();
			
			foreach(AuthorizationInfo authInfo in authInfos)
				authHandles.Add(authInfo.Handle);
			
			return authHandles.ToArray();
		}
	}
}
