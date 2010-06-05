// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Basic;
using Iaik.Utils.Serialization;
using Iaik.Tc.TPM.Lowlevel.Data;
using System.IO;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Library.Basic
{

	/// <summary>
	/// Contains the counter value
	/// </summary>
	[TypedStreamSerializable("TPMCounterValue")]
	public class TPMCounterValueCore : TPMCounterValue, ITPMBlobReadable
	{

		public static TPMCounterValueCore CreateFromTPMBlob(TPMBlob blob)
		{
			TPMCounterValueCore counterValue = new TPMCounterValueCore();
			counterValue.ReadFromTpmBlob(blob);
			return counterValue;
		}
		
		private TPMCounterValueCore()
		{
		}
		
		public TPMCounterValueCore(Stream src)
			:base(src)
		{
		}
		
		#region ITPMBlobReadable implementation
		public void ReadFromTpmBlob (TPMBlob blob)
		{
			_structureTag = (TPMStructureTag)blob.ReadUInt16();
			_label = blob.ReadBytes(4);
			_counterValue = blob.ReadUInt32();
		}
		
		#endregion
	}
}
