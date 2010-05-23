// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Org.BouncyCastle.Crypto;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Library.Common.PCRData;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Context
{


	/// <summary>
	/// Provides a .net integrated block cipher for binding and unbinding
	/// </summary>
	public class BindBlockCipher : IAsymmetricBlockCipher
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
		/// Specifies the mode of the block cipher (encryption/decryption)
		/// </summary>
		private bool _forEncryption = false;

		/// <summary>
		/// Contains the key informations
		/// </summary>
		private TPMKey _keyInfo;
		
		/// <summary>
		/// Contains the actual encryptor for encryption mode
		/// </summary>
		private IAsymmetricBlockCipher _encryptor = null;
		
		/// <summary>
		/// Cosntructs a new BindBlockCipher with the specified arguments, the seal auth is requested from the user 
		/// on first use
		/// </summary>
		/// <param name="keyHandle"></param>
		/// <param name="session"></param>
		public BindBlockCipher (ClientKeyHandle keyHandle, TPMSession session)
		{
			_keyHandle = keyHandle;
			_session = session;
			_keyInfo = _keyHandle.KeyInfo;
			
			if(_keyInfo.KeyUsage != TPMKeyUsage.TPM_KEY_BIND)
				throw new ArgumentException(string.Format("The key '{0}' is not a binding key!", keyHandle.FriendlyName));
		}
		
		
		#region IAsymmetricBlockCipher implementation
		public void Init (bool forEncryption, ICipherParameters parameters)
		{
			_forEncryption = forEncryption;
			
			if(forEncryption)
			{
				_encryptor = _keyHandle.PublicKey.CreateRSAEncrypter();
			}
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
			
		
			if(_forEncryption)
			{
				return _encryptor.ProcessBlock(inBuf, inOff, inLen);
			}
			else
			{
//				byte[] myBuf;
//				
//				if(inOff != 0 || inLen != inBuf.Length)
//				{
//					myBuf = new byte[inLen];
//					Array.Copy(inBuf, inOff, myBuf, 0, inLen);
//				}
//				else
//					myBuf = inBuf;
//				return _keyHandle.Unseal(myBuf);
				throw new NotImplementedException("Unbinding is not implemented");
			}
				
		}
		
		
		public string AlgorithmName 
		{
			get { return "TPM_Bind"; }
		}
		
		#endregion
	}
}
