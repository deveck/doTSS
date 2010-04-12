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
	/// Combines all operations that can be performed on a tpm.
	/// To retrieve a tpm session call the select method on the tpm client
	/// </summary>
	public class TPMSession : IDisposable
	{
	
		/// <summary>
		/// Specifies the owner auth data
		/// </summary>
		public const string PARAM_AUTH_OWNER = "auth_owner";
		
		/// <summary>
		/// Specifies the srk auth data
		/// </summary>
		public const string PARAM_AUTH_SRK = "auth_srk";
		
		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private EndpointContext _ctx;

		/// <summary>
		/// Contains various parameters for this session (e.g. auth data)
		/// </summary>
		private Dictionary<string, object> _parameters = new Dictionary<string, object>();
		
		
		/// <summary>
		/// Indicates if the this session is still active
		/// </summary>
		private bool _active = true;
		
		/// <summary>
		/// Contains the unique session identifier of the tpm session
		/// </summary>
		private int _sessionIdentifier;		
		
		/// <summary>
		/// Returns the unique session identifier
		/// </summary>
		public int SessionIdentifier
		{
			get { return _sessionIdentifier;}
		}
		
		/// <summary>
		/// Associated TPMClient
		/// </summary>
		private TPMClient _tpmClient;
		
		private TPMCapabilityClient _capabilityClient;
		
		/// <summary>
		/// Returns the capability client of this TPMSession
		/// </summary>
		public TPMCapabilityClient CapabilityClient
		{
			get { return _capabilityClient; }
		}
		
		private TPMEndorsementKeyHandlingClient _endorsementKeyHandling;
		
		/// <summary>
		/// Returns the endorsement key handler for this session
		/// </summary>
		public TPMEndorsementKeyHandlingClient EndorsementKeyHandling
		{
			get { return _endorsementKeyHandling; }
		}
			
		private TPMAdministrationClient _administrationClient;
		
		/// <summary>
		/// Returns the administration client for this session
		/// </summary>
		public TPMAdministrationClient AdministrationClient
		{
			get { return _administrationClient;}
		}
		
		public TPMSession (EndpointContext ctx, int sessionIdentifier, TPMClient tpmClient)
		{
			_ctx = ctx;
			_sessionIdentifier = sessionIdentifier;
			_tpmClient = tpmClient;
			_capabilityClient = new TPMCapabilityClient (this);
			_endorsementKeyHandling = new TPMEndorsementKeyHandlingClient (this);
			_administrationClient = new TPMAdministrationClient (this);
		}
		
		internal TPMCommandResponse DoTPMCommandRequest (TPMCommandRequest commandRequest)
		{
			TPMRequest request = new TPMRequest (_ctx);
			request.TPMIdentifier = _sessionIdentifier;
			request.CommandRequest = commandRequest;
			
			TPMResponse response = request.TypedExecute ();
			response.AssertTPMSuccess ();
			return response.CommandResponse;
		}

		#region IDisposable implementation
		public void Dispose ()
		{
			_tpmClient.RemoveTPMSession (this);
			_active = false;
		}
		
		#endregion
		
		#region Parameter functions
		/// <summary>
		/// Sets the value with the specified key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetValue (string key, object value)
		{
			if (_parameters.ContainsKey (key))
				_parameters[key] = value;
			else
				_parameters.Add (key, value);
		}
		
		/// <summary>
		/// Gets the value with the specified key and of the specified type
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue">is returned if the specified key is not defined n the parameters</param>
		/// <returns></returns>
		public T GetValue<T> (string key, T defaultValue)
		{
			if (_parameters.ContainsKey (key))
				return (T)_parameters[key];
			else
				return defaultValue;
		}
		
		/// <summary>
		/// Removes the value with the specified key
		/// </summary>
		/// <param name="key"></param>
		public void ClearValue (string key)
		{
			_parameters.Remove (key);
		}
		#endregion
	}
}
