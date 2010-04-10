// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils.Serialization;

namespace Iaik.Tc.TPM.Library.Common.Handles.Authorization
{

	[TypedStreamSerializable("AuthHandle")]
	public class AuthHandle : AutoStreamSerializable, ITPMHandle
	{

//		public enum AuthType
//		{
//			OIAP,
//			OSAP
//		}
//		
		/// <summary>
		/// Specifies the authorization type this handle represents
		/// </summary>
//		[SerializeMe(0)]
//		protected AuthType _authType;
//		
		/// <summary>
		/// TPM_AUTHHANDLE of the authorization session
		/// </summary>
		[SerializeMe(1)]
		protected uint _authHandle;
		

		/// <summary>
		/// Last nonce received from the TPM
		/// </summary>
		[SerializeMe(2)]
		protected byte[] _nonceEven;
	
		protected AuthHandle()
		{
		}
		
		public AuthHandle (uint authHandle)
		{
			//_authType = authType;
			_authHandle = authHandle;
		}
		
		/// <summary>
		/// Updates the TPM-received nonce
		/// </summary>
		/// <param name="nonce">new nonce</param>
		public void UpdateNonceEven (byte[] nonce)
		{
			_nonceEven = nonce;
		}
		
		#region ITPMHandle implementation
		
		public uint Handle 
		{
			get { return _authHandle; }
		}
		
		public TPMResourceType ResourceType 
		{
			get { return TPMResourceType.TPM_RT_AUTH; }
		}
		
		#endregion
	}
}