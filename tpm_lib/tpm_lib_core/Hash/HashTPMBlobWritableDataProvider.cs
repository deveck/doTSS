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
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Library.Hash
{


	public class HashTPMBlobWritableDataProvider : HashDataProvider
	{

		/// <summary>
		/// Provides the hashdata of the generated TPMBlob to the outside
		/// </summary>
		private HashStreamDataProvider _subDataProvider;
		
		
		public HashTPMBlobWritableDataProvider (ITPMBlobWritable blobWritable)
		{
			TPMBlob tempBlob = new TPMBlob ();
			blobWritable.WriteToTpmBlob (tempBlob);
			
			_subDataProvider = new HashStreamDataProvider (tempBlob, null, null, true);
		}
		
		public override int NextBytes (byte[] buffer)
		{
			return _subDataProvider.NextBytes (buffer);
		}

		
		public override void Dispose ()
		{
			base.Dispose ();
			
			_subDataProvider.Dispose ();
		}

	}
}
