
using System;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Library
{

	/// <summary>
	/// Implemented by classes that can serilize to tpm blobs
	/// </summary>
	public interface ITPMBlobWritable
	{
		void WriteToTpmBlob(TPMBlob blob);
	}
}
