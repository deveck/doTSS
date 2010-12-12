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
using Iaik.Tc.TPM.Library.Common.PCRData;
using System.Security.Cryptography;
using Iaik.Utils.Hash;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Context
{


	public class QuoteSigner : ISigner
	{
		/// <summary>
		/// Indicates if this signer is in signature generation (true) or verification (false) mode
		/// </summary>
		private bool _forSigning;

		private TPMSession _tpmSession;		
		private ClientKeyHandle _keyHandle;
		private TPMPCRSelection _pcrSelection;
		private HashAlgorithm _hashAlgorithm = null;
		
		public QuoteSigner (TPMSession tpmSession, ClientKeyHandle keyHandle, 
                    TPMPCRSelection pcrSelection)
		{
			_tpmSession = tpmSession;
			_keyHandle = keyHandle;
			_pcrSelection = pcrSelection;
		}

		
		#region ISigner implementation
		public void Init (bool forSigning, ICipherParameters parameters)
		{
			_forSigning = forSigning;
			_hashAlgorithm = _keyHandle.CreateCompatibleHashAlgorithm();
		}
		
		
		public void Update (byte input)
		{
			BlockUpdate(new byte[]{input}, 0, 1);
		}
		
		
		public void BlockUpdate (byte[] input, int inOff, int length)
		{
			_hashAlgorithm.TransformBlock(input, inOff, length, input, inOff);
		}
		
		private QuoteResponse DoQuote()
		{
	
			_hashAlgorithm.TransformFinalBlock(new byte[0], 0, 0);
			return _keyHandle.Quote(_pcrSelection, _hashAlgorithm.Hash);
		}
		
		public byte[] GenerateSignature ()
		{
			if(!_forSigning)
				throw new NotSupportedException("QuoteSigner was initialized for signature verification");

			return DoQuote().Signature;			
		}
		
		
		public bool VerifySignature (byte[] signature)
		{
			if(_forSigning)
				throw new NotSupportedException("QuoteSigner was initialized for signature generation");

			QuoteResponse quoteResponse = DoQuote();
			return ByteHelper.CompareByteArrays(signature, quoteResponse.Signature);
		}
		
		
		public void Reset ()
		{
			Init(_forSigning, null);
		}
		
		
		public string AlgorithmName 
		{
			get { return "SHA1"; }
		}
		
		#endregion

	}
}
