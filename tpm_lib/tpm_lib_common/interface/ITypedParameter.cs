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
using System.IO;

namespace Iaik.Tc.TPM.Library.Common
{	
	
	public interface ITypedParameter : ITypedStreamSerializable
	{		
	}
	
	[TypedStreamSerializable("p")]
	public class TypedPrimitiveParameter : TypedPrimitive, ITypedParameter
	{
		public TypedPrimitiveParameter (Stream src)
			:base(src)
		{
		}
		
		public TypedPrimitiveParameter (object value)
			:base(value)
		{
		}
	}
}
