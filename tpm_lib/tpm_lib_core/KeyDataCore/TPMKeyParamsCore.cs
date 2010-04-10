
using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Library.KeyDataCore
{
	
	
	
	public class TPMKeyParamsCore : TPMKeyParams, ITPMBlobReadable, ITPMBlobWritable
	{
		
		public static TPMKeyParamsCore Create (TPMAlgorithmId algorithmId, TPMEncScheme encScheme, TPMSigScheme sigScheme,
			ITypedStreamSerializable algorithmKeyParams)
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
		
		private TPMKeyParamsCore ()
		{
		}
		
		public TPMKeyParamsCore (TPMBlob src)
		{
			ReadFromTpmBlob (src);
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
					_params = new TPMRSAKeyParamsCore (paramSrc);
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
