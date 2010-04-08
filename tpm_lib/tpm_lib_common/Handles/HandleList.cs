
using System;
using Iaik.Utils.Serialization;
using System.Collections.Generic;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.Common.Handles
{

	[TypedStreamSerializable("handle_list")]
	public class HandleList : ITypedParameter, IEnumerable<uint>
	{
		
		/// <summary>
		/// Contains all handles
		/// </summary>
		protected List<uint> _handles = new List<uint>();
		
		protected HandleList ()
		{
		
		}
		
		public HandleList (Stream src)
		{
			Read (src);
		}
		
		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
		
		#endregion
		
		#region IEnumerable<ITPMHandle> implementation
		public IEnumerator<uint> GetEnumerator ()
		{
			return _handles.GetEnumerator ();
		}
		
		#endregion
		#region IStreamSerializable implementation
		public void Write (Stream sink)
		{
			StreamHelper.WriteInt32 (_handles.Count, sink);
			
			foreach (uint handle in _handles)
				StreamHelper.WriteUInt32 (handle, sink);
			
		}
		
		
		public void Read (Stream src)
		{
			int count = StreamHelper.ReadInt32 (src);
			
			for (int i = 0; i < count; i++)
				_handles.Add (StreamHelper.ReadUInt32(src));
				
		}
		
		#endregion
	}
}
