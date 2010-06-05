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
using Iaik.Tc.TPM.Keystore;
using Iaik.Tc.TPM.Library.Common.PCRData;

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

		public EndpointContext EndpointCtx
		{
			get{ return _ctx; }
		}



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
		
		
		private TPMKeystoreProvider _keystore = null;
		
		/// <summary>
		/// Gets or Sets the keystore used for this tpm session
		/// </summary>
		public TPMKeystoreProvider Keystore
		{
			get{ return _keystore; }
			set{ _keystore = value; }
		}
		
		private TPMEndorsementKeyHandlingClient _endorsementKeyHandling;
		
		/// <summary>
		/// Returns the endorsement key handler for this session
		/// </summary>
		public TPMEndorsementKeyHandlingClient EndorsementKeyHandling
		{
			get { return _endorsementKeyHandling; }
		}
		
		private TPMIntegrityClient _integrityClient;
		
		/// <summary>
		/// Returns the integrity client (pcr, ...) for this session
		/// </summary>
		public TPMIntegrityClient IntegrityClient
		{
			get { return _integrityClient; }
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
		
		private CounterClient _counterClient;
		
		/// <summary>
		/// Handles the creation and management of monotonic counters
		/// </summary>
		public CounterClient CounterClient
		{
			get{ return _counterClient; }
		}
		
		/// <summary>
		/// Provides a simple way of geting session-unique ids for general purpose
		/// </summary>
		private uint _currentHighestId = 0;
		
		private object _idLock = new object();
		
		public TPMSession (EndpointContext ctx, int sessionIdentifier, TPMClient tpmClient)
		{
			_ctx = ctx;
			_sessionIdentifier = sessionIdentifier;
			_tpmClient = tpmClient;
			_capabilityClient = new TPMCapabilityClient (this);
			_endorsementKeyHandling = new TPMEndorsementKeyHandlingClient (this);
			_administrationClient = new TPMAdministrationClient (this);
			_keyClient = new TPMKeyClient(this);
			_integrityClient = new TPMIntegrityClient(this);
			_counterClient = new CounterClient(this);
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

		/// <summary>
		/// Creates a new random number generator for this tpm
		/// </summary>
		/// <returns></returns>
		public Random CreateRNG()
		{
			return new RNG(this);
		}


		public TPMPCRSelection CreateEmptyPCRSelection()
		{
			uint pcrCount = CapabilityClient.GetPCRCount();
			
			for(ushort i = (ushort)(pcrCount/8); i>0; i--)
			{
				if(CapabilityClient.SupportsSizeOfPcr(i))
					return TPMPCRSelection.CreatePCRSelection((uint)(i * 8));
			}
			
			//Normally v1.2 TPMs should support at least selectSize == 3 and
			//v1.1 TPMS should support at least selectSize == 2
			//For any reason this method here does not work on all TPMs (maybe a bug??)
			//so we fall back to selectSize == 2 which should be supported on both versions
			return TPMPCRSelection.CreatePCRSelection(16);
			//throw new NotSupportedException("Could not find a valid selectSize parameter for this tpm");
		}
		
		/// <summary>
		/// Gets a session unique id. Once an id is taken for a session it will not appear twice in the same session
		/// </summary>
		/// <returns></returns>
		public uint GetFreeId()
		{
			lock(_idLock)
			{
				_currentHighestId ++;
				return _currentHighestId;
			}
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
		
		/// <summary>
		/// Lists all added keys
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> ListValueKeys()
		{
			return _parameters.Keys;			
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
			else if(keyInfo.KeyType == HMACKeyInfo.HMACKeyType.KeyUsageSecret)
			{
				string friendlyName = keyInfo.Parameters.GetValueOf<string>("identifier");
				bool identifierIsFriendlyName = keyInfo.Parameters.GetValueOf<bool>("identifierIsFriendlyName", false);
				
				if(!identifierIsFriendlyName)
				{
					if(_keystore.ContainsIdentifier(friendlyName) == false)
						throw new ArgumentException(string.Format("Requests for secret for key not in keystore! identifier: {0}", 
							friendlyName));

					friendlyName = _keystore.IdentifierToFriendlyName(friendlyName);
				}			
			
				dictKey = "usage_" + friendlyName;
			}
			else if(keyInfo.KeyType == HMACKeyInfo.HMACKeyType.SealAuth)
			{
				string friendlyName = keyInfo.Parameters.GetValueOf<string>("identifier");
				
				bool identifierIsFriendlyName = keyInfo.Parameters.GetValueOf<bool>("identifierIsFriendlyName", false);
				
				if(!identifierIsFriendlyName)
				{
					if(_keystore.ContainsIdentifier(friendlyName) == false)
						throw new ArgumentException(string.Format("Requests for secret for key not in keystore! identifier: {0}", 
							friendlyName));

					friendlyName = _keystore.IdentifierToFriendlyName(friendlyName);
				}			
				
				dictKey = "seal_" + friendlyName;
			}
			else if(keyInfo.KeyType == HMACKeyInfo.HMACKeyType.CounterSecret)
			{
				dictKey = "counter";
			}
			else
				throw new NotSupportedException(string.Format("The key type '{0}' is not supported", keyInfo.KeyType));
			
			
			
			
			ProtectedPasswordStorage pw = GetValue<ProtectedPasswordStorage>("secret_" + dictKey, null);
			if(pw == null)
			{
				_logger.DebugFormat("Secret for dictkey '{0}' was not found in cache, requesting from user", dictKey);
				ProtectedPasswordStorage password = RaiseRequestSecret(keyInfo);
				SetValue("secret_" + dictKey, password);
				return password;				
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
