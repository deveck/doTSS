// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Subsystems;

namespace Iaik.Tc.TPM.Subsystems
{


	public class SubsystemResponseException : Exception
	{
		private SubsystemResponse _response;
		
		public SubsystemResponse Response
		{
			get{ return _response; }
		}
		
		private int _errorCode;
		
		public int ErrorCode
		{
			get{ return _errorCode; }
		}
		
		public SubsystemResponseException (SubsystemResponse response, int errorCode, string message)
			:base(message)
		{
			_response = response;
			_errorCode = errorCode;
		}
		
		
		public override string ToString ()
		{
			return string.Format("{0}-{1} ({2})", ErrorCode, Message, StackTrace);
		}

	}
}
