// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.Common
{
	/// <summary>
	/// Represents the command request for a specific TPM command, that should
	/// be processed by the TPM library.
	/// </summary>
	public class TPMCommandRequest : ATPMCommandQuery
	{

		private TPMCommandRequest () : base()
		{
		}
		
		public TPMCommandRequest (Stream src)
		{
			Read (src);
		}
		
		public TPMCommandRequest(String commandIdentifier, Parameters param) : base (commandIdentifier, param)
		{
		}

		public override string ToString ()
		{
			return string.Format("[TPMCommandRequest cmd={0} id={1}]", CommandIdentifier, GetHashCode());
		}

		
	}
}
