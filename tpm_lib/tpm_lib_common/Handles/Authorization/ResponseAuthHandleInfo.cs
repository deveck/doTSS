// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using System.Collections.Generic;
using System.IO;

namespace Iaik.Tc.TPM.Library.HandlesCore.Authorization
{

	/// <summary>
	/// Reads the response authorization handle information 
	/// from a tpm response
	/// </summary>
	public class ResponseAuthHandleInfo
	{
		
		
		protected byte[] _nonceEven;
		
		public byte[] NonceEven
		{
			get{ return _nonceEven;}
		}
		
		protected bool _continueAuthSession;
		
		public bool ContinueAuthSession
		{
			get{ return _continueAuthSession;}
		}
		
		protected byte[] _tpmAuthData;
		
		public byte[] TpmAuthData
		{
			get{ return _tpmAuthData;}
		}
	
		protected ResponseAuthHandleInfo ()
		{
		}
	}
}
