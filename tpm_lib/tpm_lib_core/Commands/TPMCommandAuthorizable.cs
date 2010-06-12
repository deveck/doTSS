// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.HandlesCore.Authorization;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils;
using System.IO;
using log4net;
using System.Threading;

namespace Iaik.Tc.TPM.Library.Commands
{


	public abstract class TPMCommandAuthorizable : TPMCommand, IAuthorizableCommand
	{
		
		
		
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
			
		
		public virtual string GetHandle (AuthSessionNum authSessionNum)
		{
			return null;
		}
		
		
		public virtual TPMEntityTypeLSB GetEntityType (AuthSessionNum austhSessionNum)
		{
			return TPMEntityTypeLSB.TPM_ET_KEYHANDLE;
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
				_log.DebugFormat("Running command '{0}'", this);
				return InternalProcess();
			}
			catch(Exception)
			{
				//On error all used authorization handles get destroyed on the tpm,
				//so we need to remove them from our local cache
				if(typeof(IAuthorizableCommand).IsAssignableFrom(this.GetType()))
				{
					_commandAuthHelper.DestroyAuthorizationHandles((IAuthorizableCommand)this);
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
			_log.DebugFormat("Finished command '{0}'", this);
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
					_commandAuthHelper.DestroyAuthorizationHandle(this, _currentAuthorizationInfos[i].Handle);
				else 
				{
					_currentAuthorizationInfos[i].Handle.UpdateNonceEven(current.NonceEven);
				}
			}
			
		}	
		
		protected void AuthorizeMe(TPMBlob requestBlob)
		{
			_currentAuthorizationInfos = _commandAuthHelper.AuthorizeCommand(this);
		}
		
		protected override TPMBlob TransmitMe(TPMBlob requestBlob)
		{
			using(_commandAuthHelper.AcquireLock())
			{
				try
				{
					TPMBlob responseBlob;
					//Make sure that all Authorization handles are loaded
					_commandAuthHelper.LoadAuthorizationHandles(_currentAuthorizationInfos);
					
					requestBlob.Seek(0, SeekOrigin.End);
					foreach(AuthorizationInfo authInfo in _currentAuthorizationInfos)			
						WriteAuthorizationInfo (requestBlob, authInfo);
						
					lock(_tpmProvider)
					{
						
						try
						{
							_log.DebugFormat("Processing {0}", this);
							_log.DebugFormat("AuthHandles: ");
							foreach(AuthorizationInfo authInfo in _currentAuthorizationInfos)
								_log.DebugFormat(authInfo.Handle.ToString());
							_log.DebugFormat("Before execution: {0}", GetCommandInternalsBeforeExecute());
		
							responseBlob = _tpmProvider.TransmitAndCheck(requestBlob);
						}
						catch(Exception)
						{
							_log.DebugFormat("{0} FAILED", this);
							throw;
						}
						finally
						{
							_log.DebugFormat("Processed {0}", this);
						}
					}
					
					ReleaseAuthHandles(false, responseBlob);
					return responseBlob;
				}
				catch(Exception)
				{
					ReleaseAuthHandles(true, null);
					throw;
				}
			}
		}
		
		/// <summary>
		/// Checks all used authorization handles if they should be localy destroyed or not
		/// </summary>
		/// <param name="forceRelease">true to force the deletion (on error)
		/// </param>
		protected void ReleaseAuthHandles(bool forceRelease, TPMBlob responseBlob)
		{
			if(forceRelease)
			{
				_commandAuthHelper.DestroyAuthorizationHandles(this);
			}
			else
			{
				ResponseAuthHandleInfo[] responseAuthHandles = ResponseAuthHandleInfoCore.ReadAuthHandleInfos(this, responseBlob);
				
				
				for(int i = 0; i< responseAuthHandles.Length; i++)
				{
					if(responseAuthHandles[i].ContinueAuthSession)
					{
						_currentAuthorizationInfos[i].Handle.UpdateNonceEven(responseAuthHandles[i].NonceEven);
					}
					else
						_commandAuthHelper.RemoveAuthorizationHandle(this, _currentAuthorizationInfos[i].Handle);
				}
			}	
			
		}

		protected TPMBlob AuthorizeMeAndTransmit(TPMBlob requestBlob)
		{
			AuthorizeMe(requestBlob);
			return TransmitMe(requestBlob);
		}
				
                                               
		
	}
}
