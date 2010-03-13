//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Utils.CommonAttributes
{

	/// <summary>
	/// Provides a general attribute for attaching an identifier
	/// to a class
	/// </summary>
	public class ClassIdentifierAttribute : Attribute
	{
		private string _identifier;
		
		public string Identifier
		{
			get{ return _identifier; }
		}
		
		
		public ClassIdentifierAttribute (string identifier)
		{
			_identifier = identifier;
		}
	}
}
