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
				throw new NotImplementedException("Unseal not implemented");
		}
		
		
		public string AlgorithmName 
		{
			get { return "TPM_Seal"; }
		}
		
		#endregion
	}
}
