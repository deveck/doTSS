
using System;
using Iaik.Utils.Serialization;
using System.Collections.Generic;
using System.IO;
using Iaik.Utils;
using Iaik.Tc.TPM.Library.Common.Handles;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Library.HandlesCore
{

	[TypedStreamSerializable("handle_list")]
	public class HandleListCore : HandleList, ITPMBlobReadable
	{
		/// <summary>
		/// Identifies the handles that are created for this handle list
		/// </summary>
		//private TPMResourceType _resourceType;
		
		public HandleListCore (TPMBlob blob, TPMResourceType resourceType)
		{
			//_resourceType = resourceType;
			ReadFromTpmBlob (blob);
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			UInt16 handleCount = blob.ReadUInt16 ();
			
			for (int i = 0; i < handleCount; i++)
				_handles.Add (blob.ReadUInt32());
		}
		
		#endregion
		
		
	}
}
