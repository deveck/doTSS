// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections;
using Iaik.Tc.Tpm.lowlevel;
using Iaik.Tc.Tpm.library.common.interface;

namespace Iaik.Tc.Tpm.library.commands
{
	public abstract class TpmCommand : IDisposable
	{
		private readonly String name_;
		protected readonly UInt32 commandOrdinal_;
		protected readonly UInt32 commandTag_;
		
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
		private TpmCommand(UInt32 tag, UInt32 ordinal)
		{
			commandTag_ = tag;
			commandOrdinal_ = ordinal;
		}
		public abstract void process(IDictionary Parameters);
		public abstract void process();
		public abstract void clear();
		public abstract TpmBlob toBlob();
		// TODO: How to create HMAC?
				
	}
		
}