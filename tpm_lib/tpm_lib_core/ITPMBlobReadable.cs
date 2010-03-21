
using System;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Library
{

	/// <summary>
	/// Implemented by classes that can be build from tpm blobs
	/// </summary>
	public interface ITPMBlobReadable
	{
		/// <summary>
		/// Reads the data from the blob stream
		/// </summary>
		/// <param name="blob"></param>
		void ReadFromTpmBlob(TPMBlob blob);
	}
}
