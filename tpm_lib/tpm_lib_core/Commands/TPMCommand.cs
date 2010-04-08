// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Tc.TPM.Library.Common;

namespace Iaik.Tc.TPM.Library.Commands
{
	public abstract class TPMCommand //: IDisposable
	{
		protected TPMProvider _tpmProvider = null;
		
		private readonly String name_;
		protected readonly UInt32 commandOrdinal_;
		protected readonly UInt32 commandTag_;
		protected  Parameters _params;
		
		
		
		public UInt32 commandOrdinal
		{
			get
			{
				return commandOrdinal_;
			}
		}
		
		public UInt32 commandTag
		{
			get
			{
				return commandTag_;
			}
		}
		
		protected TPMCommand(){}
		protected TPMCommand(UInt32 tag, UInt32 ordinal)
		{
			commandTag_ = tag;
			commandOrdinal_ = ordinal;
		}
		private TPMCommand (UInt32 tag, UInt32 ordinal, Parameters param) : this(tag, ordinal)
		{
			_params = param;
		}
		
		public virtual void Init (Parameters param, TPMProvider tpmProvider)
		{
			_tpmProvider = tpmProvider;
			_params = param;
		}
		
		
		//public abstract void Process(Parameters param);
		//public abstract void Process();
		public abstract TPMCommandResponse Process();
		public abstract void Clear();
		//public abstract TpmBlob ToBlob();
		// TODO: How to create HMAC?
				
	}
		
}
