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
	/// Starts the authentication process using the selected AuthenticationMechanism
	/// </summary>
	public class AuthenticateRequest : TypedSubsystemRequest<AuthenticateResponse>
	{
		
		public override string Subsystem 
		{
			get { return SubsystemConstants.SUBSYSTEM_AUTH; }
		}

		public override ushort RequestIdentifier
		{
			get
			{
				return (ushort)AuthenticationSubsystem.AuthenticationRequests.Authenticate;
			}
		}
					

		public AuthenticateRequest (EndpointContext ctx)
			:base(ctx)
		{
		}
	}
	
	
	/// <summary>
	/// Response to a <see>AuthenticateRequest</see>,
	/// sends back the status of an authentication process.
	/// </summary>
	/// <remarks>
	/// This response can be extended to send an AuthenticationContext identifier. So
	/// more that one authentication sessions could be active at the same time 
	///</remarks>
	public class AuthenticateResponse : AuthenticationSubsystemResponseBase
	{	
		public AuthenticateResponse(SubsystemRequest request, EndpointContext ctx) 
			:base(request, ctx)
		{
		}		
	}
}
