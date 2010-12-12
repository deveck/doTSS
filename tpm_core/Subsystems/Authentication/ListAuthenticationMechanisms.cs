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

namespace Iaik.Tc.TPM.Subsystems.Authentication
{
	/// <summary>
	/// Requests all available authentication modes for the current connection
	/// from the server
	/// </summary>
	public class ListAuthenticationMechanismsRequest : TypedSubsystemRequest<ListAuthenticationMechanismsResponse>
	{
		
		public override string Subsystem 
		{
			get { return SubsystemConstants.SUBSYSTEM_AUTH; }
		}

		public override ushort RequestIdentifier
		{
			get
			{
				return (ushort)AuthenticationSubsystem.AuthenticationRequests.
					ListAuthenticationMechanisms;
			}
		}
					

		public ListAuthenticationMechanismsRequest (EndpointContext ctx)
			:base(ctx)
		{
		}
	}
	
	
	/// <summary>
	/// Response to a <see>ListAuthenticationModeRequest</see>,
	/// sends all the available authentication mode identifiers for the current connection
	/// back to the requesting client
	/// </summary>
	public class ListAuthenticationMechanismsResponse : AuthenticationSubsystemResponseBase
	{
		private string[] _authenticationModes;
		
		/// <summary>
		/// Gets the valid authentication modes for the current connection
		/// </summary>
		public string[] AuthenticationModes
		{
			get{ return _authenticationModes; }
            set{ _authenticationModes = value;}
		}
		
		public ListAuthenticationMechanismsResponse(SubsystemRequest request, EndpointContext ctx) 
			:base(request, ctx)
		{
		}
		
		public override void Read (Stream src)
		{
			base.Read (src);
			
			int count = StreamHelper.ReadInt32(src);
			_authenticationModes = new string[count];
			
			for(int i = 0; i<count; i++)
				_authenticationModes[i] = StreamHelper.ReadString(src);
		}

		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			if(_authenticationModes == null)
				StreamHelper.WriteInt32(0, sink);
			else
			{
				StreamHelper.WriteInt32(_authenticationModes.Length, sink);
			
				foreach(string authenticationMode in _authenticationModes)
					StreamHelper.WriteString(authenticationMode, sink);
			}
			
		}

		
	}
}
