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


// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Context;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Subsystems
{

	/// <summary>
	/// Subsystem client Request with typed response type
	/// </summary>
	public abstract class TypedClientSubsystemRequest<TResponse> : TypedSubsystemRequest<TResponse> 
		where TResponse:SubsystemResponse
	{

		private int _tpmSessionIdentifier;

		/// <summary>
		/// Gets or Sets the unique identifier of the session this request belongs to
		/// </summary>
		public int TpmSessionIdentifier {
			get { return _tpmSessionIdentifier; }
			set { _tpmSessionIdentifier = value; }
		}
		
		public TypedClientSubsystemRequest (EndpointContext ctx)
			:base(ctx)
		{
		}
		
		public override void Read (Stream src)
		{
			base.Read (src);
			_tpmSessionIdentifier = StreamHelper.ReadInt32 (src);
		}

		public override void Write (Stream sink)
		{
			base.Write (sink);
			StreamHelper.WriteInt32 (_tpmSessionIdentifier, sink);
		}

		
	}
}
