
using System;
using System.Collections.Generic;

namespace Iaik.Utils
{

    /// <summary>
    /// Static class which provides some helper functions for IDictionary<,> 
    /// </summary>
	public static class DictionaryHelper
	{
        /// <summary>
        /// Gets a boolean
        /// </summary>
        /// <param name="name"></param>
        /// <param name="paramDict"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
		public static bool GetBool(string name, IDictionary<string, string> paramDict, bool defaultValue)
		{
			if(paramDict.ContainsKey(name) == false)
				return defaultValue;
			
			bool val;
			
			if(bool.TryParse(paramDict[name], out val))
				return val;
			
			return defaultValue;
		}
		
        /// <summary>
        /// Gets an integer
        /// </summary>
        /// <param name="name"></param>
        /// <param name="paramDict"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
		public static int GetInt(string name, IDictionary<string, string> paramDict, int defaultValue)
		{
			if(paramDict.ContainsKey(name) == false)
				return defaultValue;
			
			int val;
			
			if(int.TryParse(paramDict[name], out val))
				return val;
			
			return defaultValue;
		}

        /// <summary>
        /// Gets a string
        /// </summary>
        /// <param name="name"></param>
        /// <param name="paramDict"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
		public static string GetString(string name, IDictionary<string, string> paramDict, string defaultValue)
		{
			if(paramDict.ContainsKey(name) == false)
				return defaultValue;
			
		
			return paramDict[name];
		}
	}
}
