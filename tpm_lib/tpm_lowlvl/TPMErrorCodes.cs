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
			case TPMStatusCodesEnum.TPM_OWNER_SET:
				return "Owner already set";
			default:
				return "Unknown error";
			}
		}
	}
}
