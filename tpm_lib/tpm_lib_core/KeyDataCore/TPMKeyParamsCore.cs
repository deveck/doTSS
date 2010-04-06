
using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.TPM.Library.Common.KeyData;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Library.KeyDataCore
{
	
	
	
	public class TPMKeyParamsCore : TPMKeyParams, ITPMBlobReadable
	{
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

	}
}
