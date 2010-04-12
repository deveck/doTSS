
using System;

namespace Iaik.Tc.TPM.Library.Common.Handles.Authorization
{


	public class AuthorizationInfo
	{
		private AuthHandle _authHandle;
		
		/// <summary>
		/// Associated authorization handle
		/// </summary>
		public AuthHandle Handle
		{
			get { return _authHandle; }
		}
		
		
		private byte[] _authData;
		
		public byte[] AuthData
		{
			get { return _authData;}
		}
		
		private bool _continueAuthSession;
		
		public bool ContinueAuthSession
		{
			get { return _continueAuthSession;}
		}
		
		public AuthorizationInfo ()
		{
		}
	}
}
