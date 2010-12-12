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

namespace Iaik.Tc.TPM.Library.Common.Handles
{

	/// <summary>
	/// Defines a resource on the TPM
	/// </summary>
	public interface ITPMHandle : ITypedParameter
	{
		/// <summary>
		/// Gets the tpm context blob
		/// </summary>
		byte[] ContextBlob{get;}
		
		/// <summary>
		/// Forces LoadContext to use Handle as new handle (OSAP)
		/// </summary>
		bool ForceHandle{ get; }
		
		/// <summary>
		/// Gets the TPM handle identifier
		/// </summary>
		uint Handle { get; set;}
		
		/// <summary>
		/// Gets the TPM resource type of this handle
		/// </summary>
		TPMResourceType ResourceType { get; }
	}
}
