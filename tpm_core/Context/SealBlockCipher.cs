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
using Org.BouncyCastle.Crypto;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.PCRData;
using Iaik.Tc.TPM.Library.Common.KeyData;

namespace Iaik.Tc.TPM.Context
{


	/// <summary>
	/// Provides a .net integrated block cipher for sealing and unsealing
	/// </summary>
	/// <remarks>
	/// There is a problem on unsealing because the returned block size is not fixed and is larger
	/// than the expected block size because generally a TPM_STORED_DATA structure is returned which also
	/// contains the pcr values to which the encrypted value is bound.
	/// Thus when Unsealing/Unbinding use KeyHandle.ReadEncryptedBlock to get a whole encrypted block,
	/// currently there is no other workaround available
	/// </remarks>
	public class SealBlockCipher : IAsymmetricBlockCipher
	{
		
		/// <summary>
		/// The key to use for sealing
		/// </summary>
		private ClientKeyHandle _keyHandle;
		
		/// <summary>
		/// The session this seal block cipher is associated with
		/// </summary>
		private TPMSession _session;
		
		/// <summary>
		/// PCR specification for sealing
		/// </summary>
		private TPMPCRSelection _pcrSelection;
		
		/// <summary>
		/// Unique id of this seal block cipher
		/// </summary>
		private uint _myId;

		/// <summary>
		/// Specifies the mode of the block cipher (encryption/decryption)
		/// </summary>
		private bool _forEncryption = false;

		/// <summary>
		/// Contains the key informations
		/// </summary>
		private TPMKey _keyInfo;
		
		/// <summary>
		/// Cosntructs a new SealBlockCipher with the specified arguments, the seal auth is requested from the user 
		/// on first use
		/// </summary>
		/// <param name="keyHandle"></param>
		/// <param name="session"></param>
		public SealBlockCipher (ClientKeyHandle keyHandle, TPMSession session, TPMPCRSelection pcrSelection)
		{
			_keyHandle = keyHandle;
			_session = session;
			_myId = session.GetFreeId();
			_pcrSelection = pcrSelection;
			_keyInfo = _keyHandle.KeyInfo;
		}
		
		/// <summary>
		/// Constructs a new SealBlockCipher with the specified arguments and caches the specified seal auth value
		/// </summary>
		/// <param name="keyHandle"></param>
		/// <param name="session"></param>
		/// <param name="sealAuth"></param>
		public SealBlockCipher (ClientKeyHandle keyHandle, TPMSession session, TPMPCRSelection pcrSelection, ProtectedPasswordStorage sealAuth)
			:this (keyHandle, session, pcrSelection)
		{
			_session.SetValue("secret_seal_" + _keyHandle.FriendlyName + "_" + _myId.ToString(), sealAuth);
		}
		
		
		#region IAsymmetricBlockCipher implementation
		public void Init (bool forEncryption, ICipherParameters parameters)
		{
			_forEncryption = forEncryption;
		}		
		
		public int GetInputBlockSize ()
		{
			return (int)_keyInfo.AlgorithmParams.Params.InputBlockSize;
		}
		
		
		public int GetOutputBlockSize ()
		{
			return (int)_keyInfo.AlgorithmParams.Params.OutputBlockSize;
		}		
		
		public byte[] ProcessBlock (byte[] inBuf, int inOff, int inLen)
		{
			byte[] myBuf;
			
			if(inOff != 0 || inLen != inBuf.Length)
			{
				myBuf = new byte[inLen];
				Array.Copy(inBuf, inOff, myBuf, 0, inLen);
			}
			else
				myBuf = inBuf;
		
			if(_forEncryption)
			{
				return _keyHandle.Seal(_pcrSelection, myBuf);
			}
			else
			{
				return _keyHandle.Unseal(myBuf);
			}
				
		}
		
		
		public string AlgorithmName 
		{
			get { return "TPM_Seal"; }
		}
		
		#endregion
	}
}
