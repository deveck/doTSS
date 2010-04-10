
using System;
using Iaik.Utils.Hash;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Library.Hash
{


	public class HashTPMBlobWritableDataProvider : HashDataProvider
	{

		/// <summary>
		/// Provides the hashdata of the generated TPMBlob to the outside
		/// </summary>
		private HashStreamDataProvider _subDataProvider;
		
		
		public HashTPMBlobWritableDataProvider (ITPMBlobWritable blobWritable)
		{
			TPMBlob tempBlob = new TPMBlob ();
			blobWritable.WriteToTpmBlob (tempBlob);
			
			_subDataProvider = new HashStreamDataProvider (tempBlob, null, null, true);
		}
		
		public override int NextBytes (byte[] buffer)
		{
			return _subDataProvider.NextBytes (buffer);
		}

		
		public override void Dispose ()
		{
			base.Dispose ();
			
			_subDataProvider.Dispose ();
		}

	}
}
