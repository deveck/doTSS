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
			
			
        	Console.WriteLine ("i should really compute a hmac value");
        	response.Succeeded = true;
        	response.TpmAuthData = new byte[20];
        	response.Execute();
		}
        #endregion

    }
}

