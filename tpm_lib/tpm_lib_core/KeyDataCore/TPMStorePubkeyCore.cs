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
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Lowlevel.Data;
using System.IO;

namespace Iaik.Tc.TPM.Library.KeyDataCore
{

	[TypedStreamSerializable("TPM_STORE_PUBKEY")]
	public class TPMStorePubkeyCore : TPMStorePubkey, ITPMBlobReadable, ITPMBlobWritable
	{
		/// <summary>
		/// Creates an empty pubkey object with key length = 0
		/// </summary>
		/// <returns></returns>
		public static TPMStorePubkeyCore CreateEmptyPubkey ()
		{
			TPMStorePubkeyCore pubkey = new TPMStorePubkeyCore ();
			pubkey._pubkey = new Byte[0];
			return pubkey;
		}
		
		public static TPMStorePubkeyCore CreateFromTpmBlob(TPMBlob blob)
		{
			TPMStorePubkeyCore pubkey = new TPMStorePubkeyCore();
			pubkey.ReadFromTpmBlob(blob);
			return pubkey;
		}
		
		private TPMStorePubkeyCore()
		{
		}
		
		public TPMStorePubkeyCore (Stream src)
		{
			Read(src);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			uint keyLength = blob.ReadUInt32 ();
			_pubkey = blob.ReadBytes ((int)keyLength);			
		}
		
		#endregion
		#region ITPMBlobWritable implementation
		public void WriteToTpmBlob (TPMBlob blob)
		{
			blob.WriteUInt32 ((uint)_pubkey.Length);
			blob.Write (_pubkey, 0, _pubkey.Length);
		}
		
		#endregion
	}
}
