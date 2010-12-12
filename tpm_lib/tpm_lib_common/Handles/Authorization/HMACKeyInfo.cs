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



using System;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.Handles.Authorization
{

	/// <summary>
	/// Specifies informations about the key to use for a hmac operation
	/// </summary>
	public class HMACKeyInfo : AutoStreamSerializable
	{
		public const string PARAM_KEY_IDENTIFIER = "key_id";
		
		/// <summary>
		/// Defines the Type of the hmac key to use.
		/// Some types require additional information (e.g. KeyUsageSecret requires a key identifier)
		/// </summary>
		public enum HMACKeyType
		{
			/// <summary>
			/// The owner secret is used as hmac key
			/// </summary>
			OwnerSecret,
			
			/// <summary>
			/// The SRK secret is used as hmac key
			/// </summary>
			SrkSecret,
			
			/// <summary>
			/// The secret of the specified key is used as hmac key
			/// </summary>
			KeyUsageSecret,
			
			/// <summary>
			/// The secret used to migrate a key from one tpm to another
			/// </summary>
			KeyMigrationSecret,
			
			/// <summary>
			/// The shared secret established when using OSAP
			/// </summary>
			SharedOSAPSecret,
			
			/// <summary>
			/// Enc Auth value for sealed data
			/// </summary>
			SealAuth,
			
			/// <summary>
			/// Specifies the secret of a counter
			/// </summary>
			CounterSecret,
		}
		
		[SerializeMe(0)]
		private HMACKeyType _keyType;
		
		/// <summary>
		/// Returns the hmac key type to use for hmac generation
		/// </summary>		
		public HMACKeyType KeyType
		{
			get { return _keyType; }
		}
		
		[SerializeMe(1)]
		private Parameters _parameters;
		
		/// <summary>
		/// May define some parameters needed by the specified HMACKeyType
		/// </summary>
		public Parameters Parameters
		{
			get { return _parameters; }
		}
		
		public HMACKeyInfo (HMACKeyType keyType, Parameters parameters)
		{
			_keyType = keyType;
			_parameters = parameters;
		}
		
		public HMACKeyInfo (Stream src)
		{
			Read(src);
		}
		
		public override string ToString ()
		{
			return string.Format("[HMACKeyInfo: KeyType={0}, Parameters={1}]", KeyType, Parameters);
		}

	}
}
