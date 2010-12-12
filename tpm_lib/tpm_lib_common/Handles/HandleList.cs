/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */



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
		
		public int HandleCount
		{
			get{ return _handles.Count;}
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
