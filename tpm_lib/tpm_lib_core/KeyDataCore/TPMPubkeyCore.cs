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
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Utils;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;
using System.IO;

namespace Iaik.Tc.TPM.Library.KeyDataCore
{

	[TypedStreamSerializable(TPMPubkey.PARAM_TPM_PUBKEY)]
	public class TPMPubkeyCore : TPMPubkey, ITPMBlobReadable
	{
		public static TPMPubkeyCore CreateFromTPMBlob(TPMBlob src)
		{
			TPMPubkeyCore pubkey = new TPMPubkeyCore();
			pubkey.ReadFromTpmBlob(src);
			return pubkey;
		}
	
		private TPMPubkeyCore()
		{
		}
	
		public TPMPubkeyCore (Stream src)
		{
			Read(src);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_keyParams = TPMKeyParamsCore.CreateFromTPMBlob(blob);
			_publicKey = TPMStorePubkeyCore.CreateFromTpmBlob (blob);
		}
		
		#endregion
	

	}
}
