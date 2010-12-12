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
using Iaik.Utils;
using System.IO;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{


	public class ListTPMsRequest : TypedSubsystemRequest<ListTPMsResponse>
	{
		public override ushort RequestIdentifier {
			get {
				return (ushort)TPMSubsystem.TPMRequestEnum.ListTPMDevices;
			}
		}
		
		public override string Subsystem {
			get {
				return SubsystemConstants.SUBSYSTEM_TPM;
			}
		}
		

		public ListTPMsRequest (EndpointContext ctx)
			:base(ctx)
		{
		}
	}
	
	
	public class ListTPMsResponse  : TPMSubsystemResponseBase
	{
		private string[] _tpmDevices;
		
		/// <summary>
		/// Gets or sets the tpm devices that can be used/selected by the authenticated
		/// user
		/// </summary>
		public string[] TPMDevices
		{
			get { return _tpmDevices; }
			set { _tpmDevices = value; }
		}
		
		
		public ListTPMsResponse (SubsystemRequest request, EndpointContext ctx)
			: base(request, ctx)
		{
		}
		
		public override void Read (Stream src)
		{
			base.Read (src);
		
			_tpmDevices = new string[StreamHelper.ReadInt32 (src)];
			for (int i = 0; i < _tpmDevices.Length; i++)
				_tpmDevices[i] = StreamHelper.ReadString (src);
		}
		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			if (_tpmDevices == null)
				StreamHelper.WriteInt32 (0, sink);
			else
			{
				StreamHelper.WriteInt32 (_tpmDevices.Length, sink);
				foreach (string tpmDevice in _tpmDevices)
					StreamHelper.WriteString (tpmDevice, sink);
			}
		
		}


	}
}
