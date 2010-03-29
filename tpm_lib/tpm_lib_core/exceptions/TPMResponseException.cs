
using System;

namespace Iaik.Tc.TPM.Library
{


	public class TPMResponseException:Exception
	{

		public TPMResponseException (string message)
			:base(message)
		{
		}
	}
}
