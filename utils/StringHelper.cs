/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */



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
