
using System;
using Iaik.Tc.Tpm.lowlevel.data;

namespace Iaik.Tc.Tpm.library
{

	/// <summary>
	/// Implemented by classes that can serilize to tpm blobs
	/// </summary>
	public interface ITpmBlobWritable
	{
		void WriteToTpmBlob(TpmBlob blob);
	}
}
