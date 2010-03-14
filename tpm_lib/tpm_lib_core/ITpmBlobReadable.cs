
using System;
using Iaik.Tc.Tpm.lowlevel.data;

namespace Iaik.Tc.Tpm.library
{

	/// <summary>
	/// Implemented by classes that can be build from tpm blobs
	/// </summary>
	public interface ITpmBlobReadable
	{
		/// <summary>
		/// Reads the data from the blob stream
		/// </summary>
		/// <param name="blob"></param>
		void ReadFromTpmBlob(TpmBlob blob);
	}
}
