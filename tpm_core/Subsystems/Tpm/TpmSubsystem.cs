// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Context;
using Iaik.Tc.Tpm.Configuration;
using Iaik.Tc.Tpm.lowlevel;
using Iaik.Tc.Tpm.lowlevel.backends.linux;
using Iaik.Tc.Tpm.library.commands;
using System.Collections.Generic;
using Iaik.Tc.Tpm.library;

namespace Iaik.Tc.Tpm.Subsystems.Tpm
{


	public class TpmSubsystem : BaseSubsystem<TpmSubsystem.TpmRequestEnum>
	{
		public override string SubsystemIdentifier 
		{
			get { return SubsystemConstants.SUBSYSTEM_TPM; }
		}		
		
		protected ServerContext ServerContext
		{
			get { return (ServerContext)_context;}
		}

		public enum TpmRequestEnum : ushort
		{
			/// <summary>
			/// Requests a tpm operation 
			/// </summary>
			TpmRequest = 0x0001,
			
			/// <summary>
			/// Lists all available tpm devices
			/// </summary>
			ListTpmDevices,
			
			/// <summary>
			/// Selects the specified tpm device. Returns an
			/// identifier of the selected tpm back to the client.
			/// Selecting multiple tpms is supported
			/// </summary>
			SelectTpmDevice
		}		
		
		public TpmSubsystem (EndpointContext context, IConnectionsConfiguration config)
            : base(context, config)
		{
			if (typeof(ServerContext).IsAssignableFrom (context.GetType ()) == false)
				throw new ArgumentException ("TpmSubsystem requires a ServerContext");
			
			_requestExecutionInfos.Add (TpmRequestEnum.TpmRequest,
				BuildRequestExecutionInfo<TpmSubsystem, TpmRequest, TpmResponse> (HandleTpmRequest));
			
			_requestExecutionInfos.Add (TpmRequestEnum.ListTpmDevices,
				BuildRequestExecutionInfo<TpmSubsystem, ListTpmsRequest, ListTpmsResponse> (HandleListTpmsRequest));
		}
		
		public override void HandlePacket (Iaik.Connection.Packets.DataPacket packet)
		{
			base.HandlePacket (packet);
		}

		
		private void HandleTpmRequest (TpmSubsystem subsystem, RequestContext<TpmRequest, TpmResponse> requestContext)
		{
			TpmResponse response = requestContext.CreateResponse();
			//AssertUserAuthentication(
			//TODO: Check if the current connection has an associated user (already authenticated)
			//TODO: Do some permission checking here!
			
			//Just for testing
			TPMProvider tpmDevice = new TpmDevice ("/dev/tpm0");
			tpmDevice.Open ();
			
			TPMCommand cmd = TpmCommandFactory.Create (requestContext.Request.CommandRequest);
			cmd.Init (requestContext.Request.CommandRequest.Parameters, tpmDevice);
			cmd.Process ();
			tpmDevice.Close ();
		}
		
		/// <summary>
		/// Lists all available TPM devices, that can be accessed by the authenticated user
		/// </summary>
		/// <param name="subsystem"></param>
		/// <param name="requestContext"></param>
		private void HandleListTpmsRequest (TpmSubsystem subsystem, RequestContext<ListTpmsRequest, ListTpmsResponse> requestContext)
		{
			if (!AssertUserAuthentication (null, requestContext.CreateResponse ()))
				return;
			
			List<string> tpmDevices = new List<string> ();
			foreach (KeyValuePair<string, TpmContext> ctx in ServerContext.TpmContexts)
			{
				if (IsAllowedToUseTpmDevice (ctx.Key))
					tpmDevices.Add (ctx.Key);
			}
			
			ListTpmsResponse response = requestContext.CreateResponse ();
			response.TpmDevices = tpmDevices.ToArray ();
			response.Execute ();
		}
		
		/// <summary>
		/// Checks for an authenticated user, and for the specified permission entry for that user
		/// </summary>
		/// <param name="tpmSubsystemResponse"></param>
		/// <returns></returns>
		private bool AssertUserAuthentication (string pid, TpmSubsystemResponseBase tpmSubsystemResponse)
		{
			if (ServerContext.ServerAuthenticationContext == null 
				|| ServerContext.ServerAuthenticationContext.AuthenticatedPermissionMember == null)
			{
				tpmSubsystemResponse.Succeeded = false;
				tpmSubsystemResponse.SetKnownErrorCode (TpmSubsystemResponseBase.ErrorCodeEnum.NotAuthenticated);
				tpmSubsystemResponse.Execute ();
				return false;
			}
			else if (pid != null && !ServerContext.IsCurrentUserAllowed (SubsystemIdentifier, pid))
			{
				tpmSubsystemResponse.Succeeded = false;
				tpmSubsystemResponse.SetKnownErrorCode (TpmSubsystemResponseBase.ErrorCodeEnum.NotPermitted);
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
		private bool IsAllowedToUseTpmDevice (string tpmDeviceIdentifier)
		{
			return ServerContext.IsCurrentUserAllowed (SubsystemIdentifier, "select_" + tpmDeviceIdentifier);
		}
	}
}
