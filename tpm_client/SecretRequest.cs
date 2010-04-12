
using System;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Utils.Hash;
using System.Threading;

namespace Iaik.Tc.TPM
{


	public class SecretRequest
	{
		/// <summary>
		/// Information about the secret to retrieve
		/// </summary>
		private HMACKeyInfo _keyInfo;
		
		/// <summary>
		/// Gets informations about the secret to request
		/// </summary>
		public HMACKeyInfo KeyInfo
		{
			get{ return _keyInfo;}
		}
		
		/// <summary>
		/// The resulting password
		/// </summary>
		private ProtectedPasswordStorage _protectedPassword = null;
		
		/// <summary>
		/// Gets the resulting protected password
		/// </summary>
		public ProtectedPasswordStorage ProtectedPassword
		{
			get{ return _protectedPassword; }
			set{_protectedPassword = value;}
		}
		
		private AutoResetEvent _passwordReady = new AutoResetEvent(false);
		
		public AutoResetEvent PasswordReady
		{
			get{ return _passwordReady; }
		}
		
		public SecretRequest (HMACKeyInfo keyInfo)
		{
			_keyInfo = keyInfo;
		}
		
	}
}
