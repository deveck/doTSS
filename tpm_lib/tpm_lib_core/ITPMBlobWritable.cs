
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
	
	public static class TPMBlobWriteableHelper
	{
		/// <summary>
		/// Writes target to sink with the uint size preceding
		/// </summary>
		/// <param name="sink"></param>
		/// <param name="target"></param>
		public static void WriteITPMBlobWritableWithUIntSize (TPMBlob sink, ITPMBlobWritable target)
		{
			if (target == null)
			{
				sink.WriteUInt32 (0);
			}
			else
			{
				using (TPMBlob tempBlob = new TPMBlob ())
				{	
					target.WriteToTpmBlob (tempBlob);
					sink.WriteUInt32 ((uint)tempBlob.Length);
					sink.Write (tempBlob.ToArray (), 0, (int)tempBlob.Length);
				}
			}
		}
	}
}
