// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Lowlevel
{


	public class TPMResponseException : Exception
	{
		private Int64 _tpmErrorCode;
		
		public Int64 TpmErrorCode
		{
			get{ return _tpmErrorCode;}
		}
		
		private TPMBlob _blob;
		
		public TPMBlob Blob
		{
			get{ return _blob;}
		}
		
		public TPMResponseException (Int64 tpmErrorCode, string message, TPMBlob blob)
			:base(message)
		{
			_tpmErrorCode = tpmErrorCode;
			_blob = blob;
		}
		
		public override string ToString ()
		{
			return string.Format("[TPMProviderException: TpmErrorCode={0}, Blob={1}, Message={2}]", TpmErrorCode, Blob, Message);
		}

	}
}
