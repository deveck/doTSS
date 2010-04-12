
using System;
using Iaik.Tc.TPM.Library.Commands;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Utils.Nonce;
using Iaik.Tc.TPM.Library.KeyDataCore;
using Iaik.Tc.TPM.Library.CommonTPMDataTypes;
using Iaik.Utils.Hash;

namespace Iaik.Tc.TPM.Library.Commands
{


	/// <summary>
	/// Returns the public portion of the endorsement key
	/// </summary>
	[TPMCommands(TPMCommandNames.TPM_CMD_ReadPubek)]
	public class TPM_ReadPubek : TPMCommand
	{

		public override void Clear ()
		{
		}

		
		public override TPMCommandResponse Process ()
		{
			byte[] nonce = NonceGenerator.GenerateByteNonce (20);
			
			TPMBlob requestBlob = new TPMBlob ();
			requestBlob.WriteCmdHeader (TPMCmdTags.TPM_TAG_RQU_COMMAND, TPMOrdinals.TPM_ORD_ReadPubek);
			requestBlob.Write (nonce, 0, nonce.Length);
			requestBlob.WriteCmdSize ();
			
			TPMBlob responseBlob = _tpmProvider.TransmitAndCheck (requestBlob);
			responseBlob.SkipHeader ();
			
			long posStart = responseBlob.Position;
			TPMPubkeyCore pubkey = new TPMPubkeyCore (responseBlob);
			long posEnd = responseBlob.Position;
			
			Digest digest = new Digest (responseBlob, 20);
			if (digest.CompareTo (
				new HashStreamDataProvider (responseBlob, posStart, posEnd - posStart, false),
				new HashByteDataProvider (nonce)) == false)
			{
				throw new TPMResponseException ("Local digest does not match remote digest");
			}			
			
			
			Parameters responseParams = new Parameters ();
			responseParams.AddValue (TPMPubkey.PARAM_TPM_PUBKEY, pubkey);
			
			return new TPMCommandResponse (true, TPMCommandNames.TPM_CMD_ReadPubek, responseParams);
			
		}

		
	}
}
