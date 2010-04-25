// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.Commands
{


	public abstract class TPMCommandAuthorizable : TPMCommand, IAuthorizableCommand
	{
		/// <summary>
		/// Response received by the tpm after execution
		/// </summary>
		protected TPMBlob _responseBlob = null;
		
		/// <summary>
		/// Generates the authorization data for this command,
		/// or delegates the auth data generation
		/// </summary>
		protected ICommandAuthorizationHelper _commandAuthHelper;
		
		/// <summary>
		/// Saves the assigned authorization infos of the current command
		/// </summary>
		protected AuthorizationInfo[] _currentAuthorizationInfos = null;
		
		#region IAuthorizableCommand implementation
				
		public AuthorizationInfo[] AuthorizationInfos
		{
			get{ return _currentAuthorizationInfos; }
		}
		
		public abstract byte[] Digest { get; }
		
		
		public abstract byte[] ResponseDigest { get; }		
		
		
		public ResponseAuthHandleInfo[] ResponseAuthHandleInfos 
		{
			get { return ResponseAuthHandleInfoCore.ReadAuthHandleInfos(this, _responseBlob); }
		}
		
		public void SetCommandAuthorizationHelper (ICommandAuthorizationHelper commandAuthHelper)
		{
			_commandAuthHelper = commandAuthHelper;
		}
		
		
		public virtual AuthHandle GetAuthHandle (AuthSessionNum authSessionNum)
		{
			return null;
		}
		
		
		public virtual TPMEntityTypeLSB GetEntityType (AuthSessionNum austhSessionNum)
		{
			throw new NotSupportedException();
		}
		
		
		public abstract bool SupportsAuthType (AuthHandle.AuthType authType);
		
		public abstract HMACKeyInfo GetKeyInfo (AuthSessionNum authSessionNum);
		
	
		#endregion

		public TPMCommandAuthorizable ()
		{
		}
		
		
		public override TPMCommandResponse Process ()
		{
			try
			{
				return InternalProcess();
			}
			catch(Exception)
			{
				//On error all used authorization handles get destroyed on the tpm,
				//so we need to remove them from our local cache
				if(typeof(IAuthorizableCommand).IsAssignableFrom(this.GetType()))
				{
					_commandAuthHelper.RemoveAuthorizationHandle((IAuthorizableCommand)this);
				}
				throw;
			}
			finally
			{
				if(typeof(IAuthorizableCommand).IsAssignableFrom(this.GetType()))
				{
					_commandAuthHelper.ReleaseAuthorizationHandles((IAuthorizableCommand)this);
				}
			}
		}
		
		protected virtual TPMCommandResponse InternalProcess()
		{
			throw new NotImplementedException();
		}
		
		public void WriteAuthorizationInfo (TPMBlob target, AuthorizationInfo authInfo)
		{
			target.WriteUInt32 (authInfo.Handle.Handle);
			target.Write (authInfo.Handle.NonceOdd, 0, authInfo.Handle.NonceOdd.Length);
			target.WriteBool (authInfo.ContinueAuthSession);
			target.Write (authInfo.AuthData, 0, authInfo.AuthData.Length);
		}
		
		protected void CheckResponseAuthInfo()
		{
			ResponseAuthHandleInfo[] responseAuthHandles = ResponseAuthHandleInfoCore.ReadAuthHandleInfos(this, _responseBlob);
			
			if(responseAuthHandles.Length != _currentAuthorizationInfos.Length)
			{
				throw new TPMResponseException(
                   string.Format("Expected #{0} auth handles in response, but received #{1}",
                                 _currentAuthorizationInfos.Length, responseAuthHandles.Length));
			}
			
			AuthorizationInfo[] responseAuthInfos = _commandAuthHelper.GenerateResponseAuthData(this);
			
			for(int i = 0; i<responseAuthHandles.Length; i++)
			{
				if(ByteHelper.CompareByteArrays(responseAuthHandles[i].TpmAuthData, 
				                             responseAuthInfos[i].AuthData) == false)
				{
					throw new TPMResponseException("Received tpm authdata is not valid," +
						"maybe the response got modified!");
				}
			}
			
			for(int i = 0; i<_currentAuthorizationInfos.Length; i++)
			{
				ResponseAuthHandleInfo current = responseAuthHandles[i];
			
				if(current.ContinueAuthSession == false)
					_commandAuthHelper.DestroyAuthorizationHandle(_currentAuthorizationInfos[i].Handle);
				else 
				{
					_currentAuthorizationInfos[i].Handle.UpdateNonceEven(current.NonceEven);
				}
			}
			
		}	

	}
}
