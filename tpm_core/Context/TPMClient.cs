// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Subsystems.TPMSubsystem;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Context
{


	/// <summary>
	/// Client used to do tpm operations
	/// </summary>
	public class TPMClient
	{
  		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private EndpointContext _ctx;

		/// <summary>
		/// Contains all active TPMSessions
		/// </summary>
		private Dictionary<int, TPMSession> _activeSessions = new Dictionary<int, TPMSession>();
	
		/// <summary>
		/// Requests a secret from the user
		/// </summary>
		private RequestSecretDelegate _requestSecret = null;
		
		/// <summary>
		/// Lists all available TPM devices
		/// </summary>
		public string[] TPMDevices
		{
			get
			{
				ListTPMsRequest request = new ListTPMsRequest (_ctx);
				ListTPMsResponse response = request.TypedExecute ();
				
				if (response.Succeeded == false)
					throw new Exception (response.ErrorText);
				
				return response.TPMDevices;
			}
		}
		
		/// <summary>
		/// Selects the specified tpm device. 
		/// On success a TPMSession object is returned
		/// </summary>
		/// <param name="tpmIdentifier"></param>
		/// <returns></returns>
		public TPMSession SelectTPMDevice (string tpmIdentifier)
		{
			SelectTPMRequest request = new SelectTPMRequest (_ctx);
			request.TPMIdentifier = tpmIdentifier;
			
			SelectTPMResponse response = request.TypedExecute ();
			response.AssertTPMSuccess ();
			
			TPMSession session = new TPMSession (_ctx, response.TPMSessionIdentifier, this);
			session.SetRequestSecretCallback(_requestSecret);			                                 
			                                 
			_activeSessions.Add (session.SessionIdentifier, session);
			
			return session;
		
		}
		
		/// <summary>
		/// Looks for an active session with the specified sessionIdentifier
		/// </summary>
		/// <param name="sessionIdentifier"></param>
		/// <returns></returns>
		public TPMSession FindSession (int sessionIdentifier)
		{
			if (_activeSessions.ContainsKey (sessionIdentifier))
				return _activeSessions[sessionIdentifier];
			else
				return null;
		}
		
		/// <summary>
		/// Removes the specified TPMSession from the active sessions
		/// </summary>
		/// <param name="tpmSession"></param>
		public void RemoveTPMSession (TPMSession tpmSession)
		{
			if (_activeSessions.ContainsKey (tpmSession.SessionIdentifier))
			{
				_activeSessions.Remove (tpmSession.SessionIdentifier);
				tpmSession.Dispose ();
			}
		}

		
		public void SetRequestSecretCallback(RequestSecretDelegate requestSecret)
		{
			_requestSecret = requestSecret;
		}
			
        public TPMClient (EndpointContext ctx)
        {
        	_ctx = ctx;
        }
		
		
		
		
	}
}
