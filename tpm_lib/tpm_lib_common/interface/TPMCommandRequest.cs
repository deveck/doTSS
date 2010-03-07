// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;

namespace Iaik.Tc.Tpm.library.common
{


	public class TPMCommandRequest
	{

		private String name_;
		private IDictionary<String, object> parameters_;
		public TPMCommandRequest ()
		{
		}
	}
}
