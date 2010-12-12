/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Utils.Serialization;
using Iaik.Utils.Nonce;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.Handles.Authorization
{

	[TypedStreamSerializable("AuthHandle")]
	public class AuthHandle : AutoStreamSerializable, ITPMHandle
	{

		public enum AuthType
		{
			OIAP,
			OSAP,
			Unknown
		}
		
		/// <summary>
		/// Specifies the authorization type this handle represents
		/// </summary>
		[SerializeMe(0)]
		protected AuthType _authType = AuthHandle.AuthType.OIAP;
		
		public AuthType HandleAuthType
		{
			get{ return _authType; }
		}
		
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
	
		public byte[] NonceEven
		{
			get { return _nonceEven;}
		}
		
		/// <summary>
		/// Current local nonce
		/// </summary>
		[SerializeMe(3)]
		protected byte[] _nonceOdd;
		
		public byte[] NonceOdd
		{
			get { return _nonceOdd;}
		}
		
		[SerializeMe(4)]
		protected byte[] _contextBlob = null;
		
		[SerializeMe(5)]
		protected byte[] _nonceOddOSAP = null;
		
		
		public byte[] NonceOddOSAP
		{
			get{ return _nonceOddOSAP; }
		}		
						
		[SerializeMe(6)]
		protected byte[] _nonceEvenOSAP = null;
		
		public byte[] NonceEvenOSAP
		{
			get{ return _nonceEvenOSAP; }
		}
				
		[SerializeMe(7)]
		protected byte[] _sharedSecret = null;
		
		/// <summary>
		/// Gets the shared OSAP secret, or null if not generated yet
		/// </summary>
		public byte[] SharedSecret
		{
			get{ return _sharedSecret;}
			set{ _sharedSecret = value;}
		}

        [SerializeMe(8)]
        protected TPMEntityTypeLSB _entityType;

        /// <summary>
        /// Gets or Sets the type of the associated entity
        /// </summary>
        public TPMEntityTypeLSB EntityType
        {
            get { return _entityType; }
            set { _entityType = value; }
        }

        [SerializeMe(9)]
        protected uint _entityValue;

        /// <summary>
        /// Gets or sets the value of the associated entity
        /// </summary>
        /// <remarks>
        /// This value is important for restoring exported context back into the tpm
        /// especially for OSAP (and DSAP) sessions, because the associated entity 
        /// need to be available on context loading
        /// </remarks>
        public uint EntityValue
        {
            get { return _entityValue; }
            set { _entityValue = value; }
        }

		protected AuthHandle()
		{
			_nonceOdd = new byte[20];
			_nonceOddOSAP = new byte[20];
		}
		
		public AuthHandle(Stream src)
		{
			Read(src);
		}
		
		public AuthHandle (AuthType authType, uint authHandle)
			:this()
		{
			_authType = authType;
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
		
		/// <summary>
		/// Updates the TPM-received nonce OSAP
		/// </summary>
		/// <param name="nonce">new nonce</param>
		public void UpdateNonceEvenOSAP (byte[] nonce)
		{
			_nonceEvenOSAP = nonce;
		}
		
		/// <summary>
		/// Integrates the nonce even, nonce even osap and auth handle from the givn authhandle
		/// </summary>
		/// <param name="other">
		/// A <see cref="AuthHandle"/>
		/// </param>
		public void UpdateFromOtherAuthHandle(AuthHandle other)
		{
			_nonceEven = other._nonceEven;
			_nonceEvenOSAP = other._nonceEvenOSAP;
			_authHandle = other._authHandle;
		}
		
		/// <summary>
		/// Generates a new nonce odd
		/// </summary>
		public void NewNonceOdd ()
		{
			NonceGenerator.GenerateByteNonce (_nonceOdd);
		}
		
		/// <summary>
		/// Generates a new nonce odd
		/// </summary>
		public void NewNonceOddOSAP ()
		{
			NonceGenerator.GenerateByteNonce (_nonceOddOSAP);
		}
		
		#region ITPMHandle implementation
		
		public byte[] ContextBlob
		{
			get{ return _contextBlob;}
			set{ _contextBlob = value;}
		}
		
		public uint Handle 
		{
			get { return _authHandle; }
			set {_authHandle = value;}
		}
		
		public bool ForceHandle
		{
			get{ return _authType == AuthHandle.AuthType.OSAP; }
		}
		
		public TPMResourceType ResourceType 
		{
			get { return TPMResourceType.TPM_RT_AUTH; }
		}
		
		#endregion
		
		public override string ToString ()
		{
			return string.Format("[AuthHandle: HandleAuthType={0}, Handle=0x{1:X}, ForceHandle={2}, ResourceType={3}]", HandleAuthType, Handle, ForceHandle, ResourceType);
		}

	}
}
