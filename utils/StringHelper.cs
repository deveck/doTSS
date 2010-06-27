
using System;
using System.Text;

namespace Iaik.Utils
{

    /// <summary>
    /// Static class which provides some string helper methods
    /// </summary>
	public static class StringHelper
	{
        /// <summary>
        /// Intends each line of the source string by the specified indention string
        /// </summary>
        /// <param name="src"></param>
        /// <param name="indention"></param>
        /// <returns></returns>
		public static string IndentPerLine (string src, string indention)
		{
			StringBuilder destination = new StringBuilder ();
			
			foreach (string line in src.Split ('\n'))
			{
				destination.Append (indention);
				destination.Append (line);
				destination.Append ('\n');
			}
			
			return destination.ToString ();
		}
	}
}
