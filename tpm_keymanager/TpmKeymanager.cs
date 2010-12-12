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


// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Mono.Security.X509;
using Iaik.Tc.TPM.Library;

namespace Iaik.Tc.TPM.Keymanager
{


	public class TpmKeymanager : IDisposable
	{
		TPMWrapper _tpm = null;
		uint _numFreeKeyslots;
		
		bool _isDisposed = false;
		
		private TpmKeymanager ()
		{
			
		}
		
		public TpmKeymanager (TPMWrapper tpm)
		{
			_tpm = tpm;
		}
		
		private void UpdateFreeKeySlots()
		{
			lock(this){
				
				
				
			}
		}
		
		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			lock(this)
				if(_isDisposed)
					throw new ObjectDisposedException("TPMKeymanager object is disposed");
			_tpm.Dispose();
			_isDisposed = true;
		}
		
		#endregion
		
	}
}
