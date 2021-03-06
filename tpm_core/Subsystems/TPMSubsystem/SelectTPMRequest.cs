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
using Iaik.Tc.TPM.Subsystems;
using Iaik.Tc.TPM.Context;
using Iaik.Utils;
using System.IO;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{


	public class SelectTPMRequest : TypedSubsystemRequest<SelectTPMResponse>
	{
		
		private string _tpmIdentifier = null;
		
		/// <summary>
		/// Gets or sets the identifier of the tpm device to select on the server
		/// </summary>
		public string TPMIdentifier
		{
			get { return _tpmIdentifier; }
			set { _tpmIdentifier = value;}
		}
		
		public override ushort RequestIdentifier {
			get {
				return (ushort)TPMSubsystem.TPMRequestEnum.SelectTPMDevice;
			}
		}
		
		public override string Subsystem {
			get {
				return SubsystemConstants.SUBSYSTEM_TPM;
			}
		}
		
		
		
		
		public SelectTPMRequest (EndpointContext ctx)
			:base(ctx)
		{
		}
		
		public SelectTPMRequest (EndpointContext ctx, string tpmIdentifier)
			: this(ctx)
		{
			_tpmIdentifier = tpmIdentifier;
		}
			
		public override void Read (Stream src)
		{
			base.Read (src);
			
			_tpmIdentifier = StreamHelper.ReadString (src);
		}
		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			StreamHelper.WriteString (_tpmIdentifier, sink);
		}
		
	}
	
	
	public class SelectTPMResponse : TPMSubsystemResponseBase
	{
		
		private int _tpmSessionIdentifier;
		
		/// <summary>
		/// Gets or sets the session identifier for the last select operation
		/// </summary>
		public int TPMSessionIdentifier
		{
			get { return _tpmSessionIdentifier; }
			set { _tpmSessionIdentifier = value;}
		}
		
		public SelectTPMResponse (SubsystemRequest request, EndpointContext ctx)
			: base(request, ctx)
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
