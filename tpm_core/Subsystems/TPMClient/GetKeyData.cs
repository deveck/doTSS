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
	/// Is sent from the server to the client to request the data for the specified key
	/// </summary>
	public class GetKeyDataRequest : TypedClientSubsystemRequest<GetKeyDataResponse>
	{
	
		#region Subsystem specific properties
		public override string Subsystem 
		{
			get { return SubsystemConstants.SUBSYSTEM_TPMCLIENT; }
		}
		
		public override ushort RequestIdentifier 
		{
			get { return (ushort)TPMClientSubsystem.TPMClientRequestEnum.GetKeyData; }
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
				
		
		public GetKeyDataRequest(EndpointContext ctx)
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
	
	public class GetKeyDataResponse : TPMClientSubsystemResponseBase
	{
		
		private byte[] _keyData;
		
		
		/// <summary>
		/// The data for the requested key
		/// </summary>
		public byte[] KeyData
		{
			get { return _keyData;}
			set{ _keyData = value;}
		}
		
		public GetKeyDataResponse (SubsystemRequest request, EndpointContext ctx) 
			: base(request, ctx)
		{
		}

		public override void Read (Stream src)
		{
			base.Read (src);
			
			_keyData = StreamHelper.ReadBytesSafe(src);
		}

		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			StreamHelper.WriteBytesSafe(_keyData, sink);
		}

	}
}
