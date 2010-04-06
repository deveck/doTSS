
using System;
using System.Text;

namespace Iaik.Utils
{


	public static class StringHelper
	{

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
