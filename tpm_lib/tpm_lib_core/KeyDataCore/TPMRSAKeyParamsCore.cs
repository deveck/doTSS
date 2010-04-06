
using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common.KeyData;

namespace Iaik.Tc.TPM.Library.KeyDataCore
{


	/// <summary>
	/// Describes the parameters of an RSA key
	/// </summary>
	[TypedStreamSerializable("TPMRSAKeyParams")]
	public class TPMRSAKeyParamsCore : TPMRSAKeyParams, ITPMBlobReadable
	{
			
		public TPMRSAKeyParamsCore (TPMBlob src)
		{
			ReadFromTpmBlob (src);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_keyLength = blob.ReadUInt32 ();
			_numPrimes = blob.ReadUInt32 ();
			
			uint expoSize = blob.ReadUInt32 ();
			_exponent = new byte[expoSize];
			blob.Read (_exponent, 0, (int)expoSize);
		}
		
		#endregion
	}
}
