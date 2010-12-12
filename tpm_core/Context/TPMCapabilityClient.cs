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
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Subsystems.TPMSubsystem;

namespace Iaik.Tc.TPM.Context
{
	/// <summary>
	/// Performs tpm capability requests
	/// </summary>
	public class TPMCapabilityClient
	{
		/// <summary>
		/// Transmits the packets to the server
		/// </summary>
		private TPMSession _tpmSession;

		public TPMCapabilityClient (TPMSession tpmSession)
		{
			_tpmSession = tpmSession;
		}

		
		private TPMCommandResponse BuildDoVerifyRequest(string commandIdentifier, Parameters parameters)
		{
			TPMCommandRequest versionRequest = new TPMCommandRequest (TPMCommandNames.TPM_CMD_GetCapability, parameters);
			TPMCommandResponse response = _tpmSession.DoTPMCommandRequest (versionRequest);
			
			if (response.Status == false)
				throw new TPMRequestException ("An unknown tpm error occured");
			
			return response;
		}
		
		/// <summary>
		/// Retrieves Version information of the TPM
		/// </summary>
		public CapabilityData.TPMCapVersionInfo GetTPMVersion ()
		{
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_VERSION_VAL);
			parameters.AddPrimitiveType ("subCap", new byte[0]);
			
			return BuildDoVerifyRequest(TPMCommandNames.TPM_CMD_GetCapability, parameters).Parameters.GetValueOf<CapabilityData.TPMCapVersionInfo> (CapabilityData.PARAM_TPM_VERSION_INFO);
		}
		
		/// <summary>
		/// Retrieves the count of PCR registers
		/// </summary>
		/// <returns></returns>
		public uint GetPCRCount ()
		{
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY);
			parameters.AddPrimitiveType ("subCap", CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_PCR);
			
			return this.BuildDoVerifyRequest (TPMCommandNames.TPM_CMD_GetCapability, parameters)
				.Parameters.GetValueOf<uint> (CapabilityData.PARAM_PROP_PCR);
		}
		
		/// <summary>
		/// Returns if the specified sizeIfSelect (= pcrcount / 8) is supported.
		/// This is used to create valid PCRSelection objects for this tpm
		/// </summary>
		/// <param name="size">
		/// A <see cref="System.UInt16"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool SupportsSizeOfPcr(ushort size)
		{
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_SELECT_SIZE);
			parameters.AddPrimitiveType(CapabilityData.PARAM_PROP_SELECT_SIZE, size);
			
			return this.BuildDoVerifyRequest(TPMCommandNames.TPM_CMD_GetCapability, parameters)
				.Parameters.GetValueOf<bool>(CapabilityData.PARAM_PROP_SELECT_SIZE);
			
		}
		
		/// <summary>
		/// Returns the maximum number of authorization session the tpm supports, varies with time
		/// </summary>
		/// <returns></returns>
		public uint GetMaxAuthorizationSessions ()
		{
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY);
			parameters.AddPrimitiveType ("subCap", CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_AUTHSESS);
			
			return this.BuildDoVerifyRequest (TPMCommandNames.TPM_CMD_GetCapability, parameters).Parameters.GetValueOf<uint> (CapabilityData.PARAM_PROP_MAX_AUTHSESS);
		}
		
		/// <summary>
		/// Returns the maximum number of transport session the tpm supports, varies with time
		/// </summary>
		/// <returns></returns>
		public uint GetMaxTransportSessions ()
		{
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY);
			parameters.AddPrimitiveType ("subCap", CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_TRANSESS);
			
			return this.BuildDoVerifyRequest (TPMCommandNames.TPM_CMD_GetCapability, parameters).Parameters.GetValueOf<uint> (CapabilityData.PARAM_PROP_MAX_TRANSESS);
		}
		
		/// <summary>
		/// The maximum number of sessions the TPM supports
		/// </summary>
		/// <returns></returns>
		public uint GetMaxSessions ()
		{
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY);
			parameters.AddPrimitiveType ("subCap", CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_SESSIONS);
			
			return this.BuildDoVerifyRequest (TPMCommandNames.TPM_CMD_GetCapability, parameters).Parameters.GetValueOf<uint> (CapabilityData.PARAM_PROP_MAX_SESSIONS);

		}
		
		/// <summary>
		/// The maximum number of keys the tpm can load at once
		/// </summary>
		/// <returns></returns>
		public uint GetMaxKeys ()
		{
			Parameters parameters = new Parameters ();
			parameters.AddPrimitiveType ("capArea", CapabilityData.TPMCapabilityArea.TPM_CAP_PROPERTY);
			parameters.AddPrimitiveType ("subCap", CapabilityData.TPMSubCapProperty.TPM_CAP_PROP_MAX_KEYS);
			
			return this.BuildDoVerifyRequest (TPMCommandNames.TPM_CMD_GetCapability, parameters).Parameters.GetValueOf<uint> (CapabilityData.PARAM_PROP_MAX_KEYS);

		}
	}
}
