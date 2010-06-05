// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Lowlevel;

namespace Iaik.Tc.TPM.Library.Commands.Integrity
{

	/// <summary>
	/// Extends the specified PCR value and returns the new pcr value
	/// </summary>
	[TPMCommands(TPMCommandNames.TPM_CMD_Extend)]
	public class TPM_Extend : TPMCommand
	{

		public override void Init (Parameters param, TPMProvider tpmProvider, TPMWrapper tpmWrapper)
		{
			base.Init (param, tpmProvider, tpmWrapper);
			
			if(_params.IsDefined<uint>("pcr") == false)
				throw new ArgumentException("Parameter 'pcr' is not defined");
			if(_params.IsDefined<byte[]>("digest") == false)
				throw new ArgumentException("Parameter 'digest' is not defined");
		}

		
		public override TPMCommandResponse Process ()
		{
			using(TPMBlob requestBlob = new TPMBlob())
			{
				requestBlob.WriteCmdHeader(TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_Extend);
				requestBlob.WriteUInt32(_params.GetValueOf<uint>("pcr"));
				
				byte[] digest = _params.GetValueOf<byte[]>("digest");
				if(digest.Length != 20)
					throw new ArgumentException("Digest needs to be of length '20'");
				
				requestBlob.Write(digest, 0, digest.Length);
				
				_responseBlob = TransmitMe(requestBlob);
			}
			
			_responseBlob.SkipHeader();
			_responseParameters = new Parameters();
			_responseParameters.AddPrimitiveType("pcr_value", _responseBlob.ReadBytes(20));
			return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_Extend, _responseParameters);
		}

	}
}
