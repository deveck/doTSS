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


// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.TPM.Lowlevel
{


	public static class TPMStatusCodes
	{
		public enum TPMStatusCodesEnum: uint
		{
			TPM_SUCCESS = 0x00,
			TPM_AUTHFAIL = 0x01,
			TPM_DISABLED_CMD = 0x08,
			TPM_OWNER_SET = 0x14
		}
		
		public static string ErrorCodeToMessage(uint tpmErrorCode)
		{
			switch((TPMStatusCodesEnum)tpmErrorCode)
			{
			case  TPMStatusCodesEnum.TPM_SUCCESS:
				return "No error";
			case TPMStatusCodesEnum.TPM_AUTHFAIL:
				return "Authentication failed";
			case TPMStatusCodesEnum.TPM_DISABLED_CMD:
				return "The target command has been disabled";
			case TPMStatusCodesEnum.TPM_OWNER_SET:
				return "Owner already set";
			default:
				return "Unknown error";
			}
		}
	}
}
