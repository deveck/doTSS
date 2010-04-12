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

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{


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
			CloseTPMDevice
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
				
		}
		
		public override void HandlePacket (Iaik.Connection.Packets.DataPacket packet)
		{
			base.HandlePacket (packet);
		}

		
		private void HandleTPMRequest (TPMSubsystem subsystem, RequestContext<TPMRequest, TPMResponse> requestContext)
		{
			if (!AssertUserAuthentication (null, requestContext.CreateResponse ()))
				return;

			//TODO: Do some permission checking here!

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
			
			
			response = requestContext.CreateResponse ();
			try
			{
				TPMCommandResponse commandResponse;
				
				lock (tpmContext)
				{
					commandResponse = tpmContext.TPM.Process (requestContext.Request.CommandRequest, 
						new CommandAuthorizationHelper(ServerContext, requestContext.Request.TPMIdentifier));
				}
			
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
	}
}
