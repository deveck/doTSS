// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Subsystems.TPMSubsystem;
using System.Collections.Generic;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using log4net;

namespace Iaik.Tc.TPM.Context
{
	public delegate ProtectedPasswordStorage RequestSecretDelegate(HMACKeyInfo keyInfo);

	/// <summary>
	/// Combines all operations that can be performed on a tpm.
	/// To retrieve a tpm session call the select method on the tpm client
	/// </summary>
	public class TPMSession : IDisposable
	{
	
		/// <summary>
		/// Logger
		/// </summary>
		protected ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	
		
		/// <summary>
		/// Called if the framework needs a secret from the user to authenticate
		/// a request.
		/// The default value is taken from the TPMClient
		/// </summary>
		private RequestSecretDelegate _requestSecret = null;
		
		
		
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
		
		public bool Active
		{
			get{ return _active;}
		}
		
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
		
		
		private TPMKeyClient _keyClient;
		
		/// <summary>
		/// Returns the key client for this session which performs several 
		/// operations on key handles, including key creation
		/// </summary>
		public TPMKeyClient KeyClient
		{
			get{ return _keyClient; }
		}
		
		public TPMSession (EndpointContext ctx, int sessionIdentifier, TPMClient tpmClient)
		{
			_ctx = ctx;
			_sessionIdentifier = sessionIdentifier;
			_tpmClient = tpmClient;
			_capabilityClient = new TPMCapabilityClient (this);
			_endorsementKeyHandling = new TPMEndorsementKeyHandlingClient (this);
			_administrationClient = new TPMAdministrationClient (this);
			_keyClient = new TPMKeyClient(this);
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
		
		/// <summary>
		/// If not cached, the desired secret is requested from the user
		/// </summary>
		/// <param name="keyInfo"></param>
		/// <returns></returns>
		public ProtectedPasswordStorage RequestSecret(HMACKeyInfo keyInfo)
		{
			string dictKey = null;
			
			if(keyInfo.KeyType == HMACKeyInfo.HMACKeyType.OwnerSecret)
				dictKey = PARAM_AUTH_OWNER;
			else if(keyInfo.KeyType == HMACKeyInfo.HMACKeyType.SrkSecret)
				dictKey = PARAM_AUTH_SRK;
			else
				throw new NotSupportedException(string.Format("The key type '{0}' is not supported", keyInfo.KeyType));
			
			ProtectedPasswordStorage pw = GetValue<ProtectedPasswordStorage>(dictKey, null);
			if(pw == null)
			{
				_logger.DebugFormat("Secret for dictkey '{0}' was not found in cache, requesting from user", dictKey);
				return RaiseRequestSecret(keyInfo);
			}
			else
			{
				_logger.DebugFormat("Secret for dictkey '{0}' was found in cache", dictKey);
				return pw;
			}
		}
		
		private ProtectedPasswordStorage RaiseRequestSecret(HMACKeyInfo keyInfo)
		{
			if(_requestSecret == null)
				return null;
			
			return _requestSecret(keyInfo);
		}
		
		/// <summary>
		/// Sets the request secret callback
		/// </summary>
		/// <param name="requestSecret">
		/// A <see cref="RequestSecretDelegate"/>
		/// </param>
		public void SetRequestSecretCallback(RequestSecretDelegate requestSecret)
		{
			_requestSecret = requestSecret;
		}
	}
}
