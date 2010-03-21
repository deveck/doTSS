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

namespace Iaik.Tc.TPM.Subsystems.Tpm
{


	public class TPMSubsystem : BaseSubsystem<TPMSubsystem.TPMRequestEnum>
	{
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
			SelectTPMDevice
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
		}
		
		public override void HandlePacket (Iaik.Connection.Packets.DataPacket packet)
		{
			base.HandlePacket (packet);
		}

		
		private void HandleTPMRequest (TPMSubsystem subsystem, RequestContext<TPMRequest, TPMResponse> requestContext)
		{
			TPMResponse response = requestContext.CreateResponse();
			//AssertUserAuthentication(
			//TODO: Check if the current connection has an associated user (already authenticated)
			//TODO: Do some permission checking here!
			
			//Just for testing
			TPMProvider tpmDevice = new TPMDevice ("/dev/tpm0");
			tpmDevice.Open ();
			
			TPMCommand cmd = TPMCommandFactory.Create (requestContext.Request.CommandRequest);
			cmd.Init (requestContext.Request.CommandRequest.Parameters, tpmDevice);
			cmd.Process ();
			tpmDevice.Close ();
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
			foreach (KeyValuePair<string, TPMContext> ctx in ServerContext.TpmContexts)
			{
				if (IsAllowedToUseTPMDevice (ctx.Key))
					tpmDevices.Add (ctx.Key);
			}
			
			ListTPMsResponse response = requestContext.CreateResponse ();
			response.TPMDevices = tpmDevices.ToArray ();
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
				tpmSubsystemResponse.SetKnownErrorCode (TPMSubsystemResponseBase.ErrorCodeEnum.NotAuthenticated);
				tpmSubsystemResponse.Execute ();
				return false;
			}
			else if (pid != null && !ServerContext.IsCurrentUserAllowed (SubsystemIdentifier, pid))
			{
				tpmSubsystemResponse.Succeeded = false;
				tpmSubsystemResponse.SetKnownErrorCode (TPMSubsystemResponseBase.ErrorCodeEnum.NotPermitted);
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
