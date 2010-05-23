// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Runtime.InteropServices;
using Iaik.Tc.TPM.Library;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;



namespace Iaik.Tc.TPM.Library.Commands
{
	[TPMCommands(TPMCommandNames.TPM_CMD_PCRRead)]
	public sealed class TPM_PCRRead : TPMCommand
	{
		/// <summary>
		/// The PCR register that should be read
		/// </summary>
		private UInt32 _register = 0;
		
		public override void Clear ()
		{
			throw new System.NotImplementedException();
		}
		
		
		
		public TPM_PCRRead() : base(TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_PcrRead)
		{	
		}
	
		public override void Init (Parameters param, TPMProvider provider, TPMWrapper tpmWrapper)
		{
			base.Init(param, provider, tpmWrapper);
			_register = param.GetValueOf<UInt32>("pcrnum");
		}

		public override TPMCommandResponse Process ()
		{
			TPMBlob requestBlob = new TPMBlob ();
			requestBlob.WriteCmdHeader (TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_PcrRead);
			requestBlob.WriteUInt32 ((uint)_register);
			requestBlob.WriteCmdSize ();
			
			TPMBlob responseBlob = _tpmProvider.TransmitAndCheck (requestBlob);
			Parameters responseParam = new Parameters();
			
			// is done in transmit and check
			//responseBlob.ReadUInt16();
			//responseBlob.ReadUInt32();
			//UInt32 i = responseBlob.ReadUInt32();
			
			byte[] val = responseBlob.ReadBytes(20);
			
			
			responseParam.AddPrimitiveType("pcrnum", _register);
			responseParam.AddPrimitiveType("value", val);
			
			
			TPMCommandResponse response = new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_PCRRead, responseParam);
			
			return response;
		}
	
		
	}
}
