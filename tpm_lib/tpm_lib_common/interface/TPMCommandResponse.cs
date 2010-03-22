// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using Iaik.Utils;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common
{
	public sealed class TPMCommandResponse : ATPMCommandQuery
	{
	
		bool _status = false;
		
		public bool Status
		{
			get
			{
				return _status;
			}
		}
		
		public TPMCommandResponse () : base()
		{
		}
		
		public TPMCommandResponse (Stream src)
		{
			Read (src);
		}
		
		public TPMCommandResponse(bool status, String commandIdentifier, Parameters param) : base (commandIdentifier, param)
		{
			_status = status;
		}
		
		#region IStreamSerializable implementation
		public void Write (Stream sink)
		{
			base.Write(sink);
			StreamHelper.WriteBool (_status, sink);
		}
		
		public void Read (Stream src)
		{
			base.Read(src);
			_status = StreamHelper.ReadBool (src);
		}
		#endregion
	}
}
