/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Configuration;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Backends.Linux;
using Iaik.Tc.TPM.Library.Commands;
using System.Collections.Generic;
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.KeyDataCore;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{

    /// <summary>
    /// Handles all TPM related requests
    /// </summary>
    /// <remarks>
    /// Before a client can use a TPM, it needs to be selected. The client then receives an identifier for the 
    /// selected TPM, this identifier then can be used to execute tpm commands as long as the tpm device and the connection remains open
    /// </remarks>
	public class TPMSubsystem : BaseServerSubsystem<TPMSubsystem.TPMRequestEnum>
	{
		
		/// <summary>
		/// The currently highest tpm identifier
		/// </summary>
		private int _maxTPMIdentifier = 0;
		
		/// <summary>
		/// Contains all currently selected TPMS
		/// @key: tpm identifier, strictly increasing
		/// </summary>
		private IDictionary<int, TPMContext> _selectedTPMs = new Dictionary<int, TPMContext>();
		
	
		public override string SubsystemIdentifier 
		{
			get { return SubsystemConstants.SUBSYSTEM_TPM; }
		}		
		
		protected ServerContext ServerContext
		{
			get { return (ServerContext)_context;}
		}

		public enum TPMRequestEnum : ushort
		{
			/// <summary>
			/// Requests a tpm operation 
			/// </summary>
			TPMRequest = 0x0001,
			
			/// <summary>
			/// Lists all available tpm devices
			/// </summary>
			ListTPMDevices,
			
			/// <summary>
			/// Selects the specified tpm device. Returns an
			/// identifier of the selected tpm back to the client.
			/// Selecting multiple tpms is supported
			/// </summary>
			SelectTPMDevice,
			
			/// <summary>
			/// Invalidates the passed tpm device identifier
			/// </summary>
			CloseTPMDevice,
			
			/// <summary>
			/// Requests informations about a tpm key
			/// </summary>
			KeyInfo
		}		
		
		public TPMSubsystem (EndpointContext context, IConnectionsConfiguration config)
            : base(context, config)
		{
			if (typeof(ServerContext).IsAssignableFrom (context.GetType ()) == false)
				throw new ArgumentException ("TpmSubsystem requires a ServerContext");
			
			_requestExecutionInfos.Add (TPMRequestEnum.TPMRequest,
				BuildRequestExecutionInfo<TPMSubsystem, TPMRequest, TPMResponse> (HandleTPMRequest));
			
			_requestExecutionInfos.Add (TPMRequestEnum.ListTPMDevices,
				BuildRequestExecutionInfo<TPMSubsystem, ListTPMsRequest, ListTPMsResponse> (HandleListTPMsRequest));
			
			_requestExecutionInfos.Add (TPMRequestEnum.SelectTPMDevice,
				BuildRequestExecutionInfo<TPMSubsystem, SelectTPMRequest, SelectTPMResponse> (HandleSelectTPMRequest));
				
			_requestExecutionInfos.Add (TPMRequestEnum.KeyInfo,
				BuildRequestExecutionInfo<TPMSubsystem, KeyInfoRequest, KeyInfoResponse> (HandleKeyInfoRequest));
				
		}
		
		public override void HandlePacket (Iaik.Connection.Packets.DataPacket packet)
		{
			base.HandlePacket (packet);
		}

		public override void Dispose ()
		{
			base.Dispose ();
			
			//Remove all keys of this client from the keystore
			foreach(KeyValuePair<int, TPMContext> context in _selectedTPMs)
				context.Value.KeyManager.UnloadKeysOfContext(context.Key);
		}

		
		private void HandleTPMRequest (TPMSubsystem subsystem, RequestContext<TPMRequest, TPMResponse> requestContext)
		{
			if (!AssertUserAuthentication (null, requestContext.CreateResponse ()))
				return;

			


			TPMContext tpmContext;
			TPMResponse response;
			
			lock (_selectedTPMs)
			{
				if (_selectedTPMs.ContainsKey (requestContext.Request.TPMIdentifier) == false)
				{
					response = requestContext.CreateResponse ();
					response.Succeeded = false;
					response.SetKnownErrorCode (TPMSubsystemResponseBase.ErrorCodeEnum.TPMIdentifierNotValid);
					response.Execute ();
					return;
				}
				
				tpmContext = _selectedTPMs[requestContext.Request.TPMIdentifier];
			}
		
			string commandIdentifier = requestContext.Request.CommandRequest.CommandIdentifier;
			
			if(IsAllowedToRunCommand(commandIdentifier, tpmContext) == false)
			{
				response = requestContext.CreateResponse();
				response.Succeeded = false;
				response.SetKnownCommonError(SubsystemResponse.CommonErrorCodes.NotPermitted);
				response.Execute();
				return;
			}
			
			_logger.DebugFormat("Executing {0}", requestContext.Request.CommandRequest.CommandIdentifier);
			
			response = requestContext.CreateResponse ();
			try
			{
				TPMCommandResponse commandResponse;
				
				//Locking here is not a good idea, because the Process method 
				//block until the final command is executed. This could take a long time,
				//because the user might be asked to authenticate several commands
				//lock (tpmContext)
				//{
					ICommandAuthorizationHelper commandAuthHelper = new CommandAuthorizationHelper(ServerContext, requestContext.Request.TPMIdentifier, tpmContext);
					commandResponse = tpmContext.TPM.Process (requestContext.Request.CommandRequest, 
						commandAuthHelper,
						new KeyManagerHelper(ServerContext, tpmContext, requestContext.Request.TPMIdentifier, commandAuthHelper));
				//}
			
				response.CommandResponse = commandResponse;
				response.Execute ();
			}
			catch(Exception ex)
			{
				_logger.FatalFormat("Error processing TPMRequest: {0}", ex);
				response.Succeeded = false;
				response.CustomErrorMessage = ex.Message;
				response.Execute();
			}
			
			
		}
		
		/// <summary>
		/// Lists all available TPM devices, that can be accessed by the authenticated user
		/// </summary>
		/// <param name="subsystem"></param>
		/// <param name="requestContext"></param>
		private void HandleListTPMsRequest (TPMSubsystem subsystem, RequestContext<ListTPMsRequest, ListTPMsResponse> requestContext)
		{
			if (!AssertUserAuthentication (null, requestContext.CreateResponse ()))
				return;
			
			List<string> tpmDevices = new List<string> ();
			foreach (KeyValuePair<string, TPMContext> ctx in ServerContext.TPMContexts)
			{
				if (IsAllowedToUseTPMDevice (ctx.Key))
					tpmDevices.Add (ctx.Key);
			}
			
			ListTPMsResponse response = requestContext.CreateResponse ();
			response.TPMDevices = tpmDevices.ToArray ();
			response.Execute ();
		}
		
		/// <summary>
		/// Selects the specified tpm device (if the authenticated user has the permission)
		/// </summary>
		/// <param name="subsystem"></param>
		/// <param name="requestContext"></param>
		private void HandleSelectTPMRequest (TPMSubsystem subsystem, RequestContext<SelectTPMRequest, SelectTPMResponse> requestContext)
		{
			if (!AssertUserAuthentication ("select_" + requestContext.Request.TPMIdentifier, requestContext.CreateResponse ()))
				return;
			
			SelectTPMResponse response = requestContext.CreateResponse ();
			
			if (ServerContext.TPMContexts.ContainsKey (requestContext.Request.TPMIdentifier) == false)
			{
				response.Succeeded = false;
				response.SetKnownErrorCode (TPMSubsystemResponseBase.ErrorCodeEnum.TPMDeviceNotFound);
				response.Execute ();
				return;
			}
			
			
			lock (_selectedTPMs)
			{
				int myId = (++_maxTPMIdentifier);
				
				_selectedTPMs.Add (myId, ServerContext.TPMContexts[requestContext.Request.TPMIdentifier]);
				response.TPMSessionIdentifier = myId;
			}
			
			response.Execute ();
		}
		
		/// <summary>
		/// Retrieves informations about keys 
		/// </summary>
		/// <param name="subsystem"></param>
		/// <param name="requestContext"></param>
		private void HandleKeyInfoRequest (TPMSubsystem subsystem, RequestContext<KeyInfoRequest, KeyInfoResponse> requestContext)
		{
			
				
			TPMContext tpmContext;
			KeyInfoResponse response;
			
			lock (_selectedTPMs)
			{
				if (_selectedTPMs.ContainsKey (requestContext.Request.TPMIdentifier) == false)
				{
					response = requestContext.CreateResponse ();
					response.Succeeded = false;
					response.SetKnownErrorCode (TPMSubsystemResponseBase.ErrorCodeEnum.TPMIdentifierNotValid);
					response.Execute ();
					return;
				}
				
				tpmContext = _selectedTPMs[requestContext.Request.TPMIdentifier];
			}
			
			if (!AssertUserAuthentication ("key_info_" + _selectedTPMs[requestContext.Request.TPMIdentifier].DeviceName, requestContext.CreateResponse ()))
				return;
			
			KeyManagerHelper keyManagerHelper = new KeyManagerHelper(ServerContext, tpmContext, requestContext.Request.TPMIdentifier,
				new CommandAuthorizationHelper(ServerContext, requestContext.Request.TPMIdentifier, tpmContext));
			
			if(keyManagerHelper.ContainsIdentifier(requestContext.Request.KeyIdentifier) == false)
			{
				response = requestContext.CreateResponse();
				response.Succeeded = false;
				response.SetKnownErrorCode(TPMSubsystemResponseBase.ErrorCodeEnum.NotAValidKeyIdentifier);
				response.Execute();
				return;
			}
			
			byte[] keyBlob = keyManagerHelper.GetKeyBlob(requestContext.Request.KeyIdentifier);
			response = requestContext.CreateResponse();
			response.Succeeded = true;
			response.TPMKey = TPMKeyCore.CreateFromBytes(keyBlob);
			response.Execute();
		}


		/// <summary>
		/// Checks for an authenticated user, and for the specified permission entry for that user
		/// </summary>
		/// <param name="tpmSubsystemResponse"></param>
		/// <returns></returns>
		private bool AssertUserAuthentication (string pid, TPMSubsystemResponseBase tpmSubsystemResponse)
		{
			if (ServerContext.ServerAuthenticationContext == null 
				|| ServerContext.ServerAuthenticationContext.AuthenticatedPermissionMember == null)
			{
				tpmSubsystemResponse.Succeeded = false;
				tpmSubsystemResponse.SetKnownCommonError(TPMSubsystemResponseBase.CommonErrorCodes.NotAuthenticated);
				tpmSubsystemResponse.Execute ();
				return false;
			}
			else if (pid != null && !ServerContext.IsCurrentUserAllowed (SubsystemIdentifier, pid))
			{
				tpmSubsystemResponse.Succeeded = false;
				tpmSubsystemResponse.SetKnownCommonError (TPMSubsystemResponseBase.CommonErrorCodes.NotPermitted);
				tpmSubsystemResponse.Execute ();
				return false;
			}
			
			return true;
		
		}
		
		/// <summary>
		/// Checks if the current user is allowed to use the specified tpm device
		/// </summary>
		/// <param name="tpmDeviceIdentifier"></param>
		/// <returns></returns>
		private bool IsAllowedToUseTPMDevice (string tpmDeviceIdentifier)
		{
			return ServerContext.IsCurrentUserAllowed (SubsystemIdentifier, "select_" + tpmDeviceIdentifier);
		}
		
		/// <summary>
		/// Checks if the current user is allowed to run the specified command on the specified tpm
		/// </summary>
		/// <param name="commandIdentifier"></param>
		/// <param name="tpmContext"></param>
		/// <returns></returns>
		private bool IsAllowedToRunCommand(string commandIdentifier, TPMContext tpmContext)
		{
			Permission permission = ServerContext.AccessControlList.FindPermission(SubsystemIdentifier, commandIdentifier + "_" + tpmContext.DeviceName);
			
			if(permission == null)
				permission = ServerContext.AccessControlList.FindPermission(SubsystemIdentifier, commandIdentifier);
			
			
			if(permission != null)
				return permission.IsPermitted(ServerContext.ServerAuthenticationContext.AuthenticatedPermissionMember);
			
			return false;
			
		}
		
	}
}
