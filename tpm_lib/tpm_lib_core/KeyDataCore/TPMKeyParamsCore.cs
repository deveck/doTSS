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
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Library.KeyDataCore
{
	
	
	[TypedStreamSerializable("TPMKeyParams")]
	public class TPMKeyParamsCore : TPMKeyParams, ITPMBlobReadable, ITPMBlobWritable
	{
		
		public static TPMKeyParamsCore Create (TPMAlgorithmId algorithmId, TPMEncScheme encScheme, TPMSigScheme sigScheme,
			ITPMAlgorithmSpecificKeyParams algorithmKeyParams)
		{
			if (algorithmId == TPMAlgorithmId.TPM_ALG_RSA && !(algorithmKeyParams is TPMRSAKeyParamsCore))
				throw new ArgumentException (string.Format ("algorithm: {0} required TPMRSAKeyParamsCore", algorithmId));
			
			TPMKeyParamsCore keyParamsCore = new TPMKeyParamsCore ();
			keyParamsCore._algorithmId = algorithmId;
			keyParamsCore._encScheme = encScheme;
			keyParamsCore._sigScheme = sigScheme;
			keyParamsCore._params = algorithmKeyParams;

			return keyParamsCore;
		}
		
		public static TPMKeyParamsCore CreateFromTPMBlob ( TPMBlob blob)
		{
			TPMKeyParamsCore keyParams = new TPMKeyParamsCore();
			keyParams.ReadFromTpmBlob(blob);
			return keyParams;
		}
		
		private TPMKeyParamsCore ()
		{
		}
		
		public TPMKeyParamsCore (Stream src)
		{
			Read(src);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_algorithmId = (TPMAlgorithmId)blob.ReadUInt32 ();
			_encScheme = (TPMEncScheme)blob.ReadUInt16 ();
			_sigScheme = (TPMSigScheme)blob.ReadUInt16 ();
			
			UInt32 paramsSize = blob.ReadUInt32 ();
			byte[] paramsData = new byte[paramsSize];
			
			blob.Read (paramsData, 0, paramsData.Length);
			
			using (TPMBlob paramSrc = new TPMBlob (paramsData))
			{
				if (_algorithmId == TPMAlgorithmId.TPM_ALG_RSA)
					_params = TPMRSAKeyParamsCore.CreateFromTPMBlob(paramSrc);
				else if (_algorithmId == TPMAlgorithmId.TPM_ALG_AES128 ||
					_algorithmId == TPMAlgorithmId.TPM_ALG_AES192 ||
					_algorithmId == TPMAlgorithmId.TPM_ALG_AES256)
					//TODO
					throw new NotImplementedException ("Symmetric key params not implemented");		
			}
		}
		
		#endregion
		#region ITPMBlobWritable implementation
		public void WriteToTpmBlob (TPMBlob blob)
		{
			blob.WriteUInt32 ((uint)_algorithmId);
			blob.WriteUInt16 ((ushort)_encScheme);
			blob.WriteUInt16 ((ushort)_sigScheme);
			TPMBlobWriteableHelper.WriteITPMBlobWritableWithUIntSize (blob, (ITPMBlobWritable)_params);
			
		}
		
		#endregion

	}
}
