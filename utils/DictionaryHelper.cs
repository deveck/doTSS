
using System;
using System.Collections.Generic;

namespace Iaik.Utils
{


	public static class DictionaryHelper
	{
		public static bool GetBool(string name, IDictionary<string, string> paramDict, bool defaultValue)
		{
			if(paramDict.ContainsKey(name) == false)
				return defaultValue;
			
			bool val;
			
			if(bool.TryParse(paramDict[name], out val))
				return val;
			
			return defaultValue;
		}

	}
}
