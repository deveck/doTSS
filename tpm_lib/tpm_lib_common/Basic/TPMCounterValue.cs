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


// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.Basic
{

	/// <summary>
	/// Contains the counter value
	/// </summary>
	[TypedStreamSerializable("TPMCounterValue")]
	public class TPMCounterValue : AutoStreamSerializable, ITypedParameter
	{

		[SerializeMe(0)]
		protected TPMStructureTag _structureTag;

		/// <summary>
		/// Contains the label which is established on counter creation
		/// </summary>
		[SerializeMe(1)]
		protected byte[] _label;

		/// <summary>
		/// Gets the label specified on counter creation
		/// </summary>
		public byte[] Label
		{
			get{ return _label; }
		}

		
		[SerializeMe(2)]
		protected uint _counterValue;
		
		/// <summary>
		/// Returns the current value of the counter
		/// </summary>
		public uint CounterValue
		{
			get{ return _counterValue;}
		}
		
		protected TPMCounterValue ()
		{
		}
		
		public TPMCounterValue(Stream src)
		{
			Read(src);
		}
		
	}
}
