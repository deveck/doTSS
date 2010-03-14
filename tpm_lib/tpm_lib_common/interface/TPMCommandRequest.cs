// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.Tpm.library.common
{
	/// <summary>
	/// Represents the command request for a specific TPM command, that should
	/// be processed by the TPM library.
	/// </summary>
	public class TPMCommandRequest : IStreamSerializable
	{

		private String commandIdentifier_;
		private Parameters params_;
		
		public TPMCommandRequest ()
		{
		}
		
		public TPMCommandRequest (Stream src)
		{
			Read (src);
		}
		
		public TPMCommandRequest(String commandIdentifier, Parameters param)
		{
			commandIdentifier_ = commandIdentifier;
			params_ = param;
		}
		
		public Parameters Parameters
		{
			get
			{
				return params_; 
			}
			
			set
			{
				if (value == null)
					throw new ArgumentNullException ("params_", "TPMCommandRequest params_ must not be null");
				params_ = value;
			}
		}
		
		public string CommandIdentifier
		{
			get { return commandIdentifier_;}
		}
		
		#region IStreamSerializable implementation
		public void Write (Stream sink)
		{
			StreamHelper.WriteString (commandIdentifier_, sink);
			params_.Write (sink);
		}
		
		
		public void Read (Stream src)
		{
			commandIdentifier_ = StreamHelper.ReadString (src);
			params_ = new Parameters (src);
		}
		
		#endregion
	}
}
