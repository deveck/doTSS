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
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common;
using System.IO;
using Iaik.Utils;
using Iaik.Utils.Hash;

namespace Iaik.Tc.TPM.Subsystems.TPMClient
{

	/// <summary>
	/// Is sent from the server to the client to check if the specified key is contained in the loaded keystore
	/// </summary>
	public class ContainsKeyRequest : TypedClientSubsystemRequest<ContainsKeyResponse>
	{
	
		#region Subsystem specific properties
		public override string Subsystem 
		{
			get { return SubsystemConstants.SUBSYSTEM_TPMCLIENT; }
		}
		
		public override ushort RequestIdentifier 
		{
			get { return (ushort)TPMClientSubsystem.TPMClientRequestEnum.ContainsKey; }
		}
		#endregion

		
		private string _identifier = null;
		
		/// <summary>
		/// Gets information about the secret used to generate the hmac
		/// </summary>
		public string Identifier
		{
			get { return _identifier; }
			set { _identifier = value;}
		}
				
		
		public ContainsKeyRequest(EndpointContext ctx)
			:base(ctx)
		{
		}
		
		
		public override void Read (Stream src)
		{
			base.Read (src);
			
			_identifier = StreamHelper.ReadString(src);
		}

		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			StreamHelper.WriteString(_identifier, sink);			
		}

	}
	
	public class ContainsKeyResponse : TPMClientSubsystemResponseBase
	{
		
		private bool _contained;
		
		
		/// <summary>
		/// The generated hmac authdata
		/// </summary>
		public bool Contained
		{
			get { return _contained;}
			set{ _contained = value;}
		}
		
		public ContainsKeyResponse (SubsystemRequest request, EndpointContext ctx) 
			: base(request, ctx)
		{
		}

		public override void Read (Stream src)
		{
			base.Read (src);
			
			_contained = StreamHelper.ReadBool(src);
		}

		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			StreamHelper.WriteBool(_contained, sink);
		}

	}
}
