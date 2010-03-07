// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;

namespace Iaik.Tc.Tpm.library.common
{
	public class Parameters
	{
		private IDictionary<String, Object> encapsulated_ = new Dictionary<String, Object>();

		public Parameters ()
		{
		}
		
		public void AddValue(String key, Object val)
		{
			encapsulated_.Add(key, val);
		}
		
		public T GetValueOf<T>(String key)
		{
			return (T) encapsulated_[key];
		}
	}
}
