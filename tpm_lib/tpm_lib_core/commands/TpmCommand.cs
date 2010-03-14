// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections;
using Iaik.Tc.Tpm.lowlevel;
using Iaik.Tc.Tpm.lowlevel.data;
using Iaik.Tc.Tpm.library.common;

namespace Iaik.Tc.Tpm.library.commands
{
	public abstract class TpmCommand //: IDisposable
	{
		protected TPMProvider _tpmProvider = null;
		
		private readonly String name_;
		protected readonly UInt32 commandOrdinal_;
		protected readonly UInt32 commandTag_;
		private readonly Parameters params_;
		
		
		
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
		
		protected TpmCommand(){}
		protected TpmCommand(UInt32 tag, UInt32 ordinal)
		{
			commandTag_ = tag;
			commandOrdinal_ = ordinal;
		}
		private TpmCommand (UInt32 tag, UInt32 ordinal, Parameters param) : this(tag, ordinal)
		{
			params_ = param;
		}
		
		public virtual void Init (Parameters param, TPMProvider tpmProvider)
		{
			_tpmProvider = tpmProvider;
		}
		
		
		//public abstract void Process(Parameters param);
		public abstract void Process();
		public abstract void Clear();
		//public abstract TpmBlob ToBlob();
		// TODO: How to create HMAC?
				
	}
		
}
