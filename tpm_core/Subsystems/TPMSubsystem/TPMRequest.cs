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
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.TPM.Context;
using Iaik.Utils;
using Iaik.Tc.TPM.Configuration;
using Iaik.Tc.TPM.Authentication;
using System.Collections.Generic;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{
	/// <summary>
	/// Requests all available authentication modes for the current connection
	/// from the server
	/// </summary>
	public class TPMRequest : TypedSubsystemRequest<TPMResponse>
	{
		
		
		
		public override string Subsystem 
		{
			get { return SubsystemConstants.SUBSYSTEM_TPM; }
		}

		public override ushort RequestIdentifier
		{
			get	{ return (ushort)TPMSubsystem.TPMRequestEnum.TPMRequest; }
		}
				
		
		private int _tpmIdentifier;
		
		/// <summary>
		/// Identifies the TPM this action should take place on.
		/// Acquire this id by selecting the tpm device 
		/// </summary>
		public int TPMIdentifier
		{
			get { return _tpmIdentifier; }
			set { _tpmIdentifier = value;}
		}
		
		private TPMCommandRequest _commandRequest = null;
		
		/// <summary>
		/// The request with its parameters 
		/// </summary>
		public TPMCommandRequest CommandRequest
		{
			get { return _commandRequest; }
			set { _commandRequest = value;}
		}

		public TPMRequest (EndpointContext ctx)
			: base(ctx)
		{
		}
		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			StreamHelper.WriteInt32 (_tpmIdentifier, sink);
			_commandRequest.Write (sink);
		}
 
		
		public override void Read (Stream src)
		{
			base.Read (src);
			
			_tpmIdentifier = StreamHelper.ReadInt32 (src);
			_commandRequest = new TPMCommandRequest (src);
		}

	}
	
	
	/// <summary>
	/// Response to a <see>TpmRequest</see>
	/// </summary>
	public class TPMResponse : TPMSubsystemResponseBase
	{
		
		private TPMCommandResponse _commandResponse = null;
		
		/// <summary>
		/// The response with its parameters 
		/// </summary>
		public TPMCommandResponse CommandResponse
		{
			get { return _commandResponse; }
			set { _commandResponse = value;}
		}
			
		
		public TPMResponse(SubsystemRequest request, EndpointContext ctx) 
			:base(request, ctx)
		{
		}
		
		public override void Read (Stream src)
		{
			base.Read (src);
			
			if (Succeeded)
			{
				_commandResponse = new TPMCommandResponse (src);
			}
		}

		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			if (Succeeded)
			{
				_commandResponse.Write (sink);
			}
		}

		
	}
}
