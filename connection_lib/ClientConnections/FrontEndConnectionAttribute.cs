// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.CommonAttributes;

namespace Iaik.Connection.ClientConnections
{

	/// <summary>
	/// Attribute that is defined for all FrontEndConnection
	/// </summary>
	public class FrontEndConnectionAttribute : ClassIdentifierAttribute
	{
		
		public FrontEndConnectionAttribute (string connectionName)
			:base(connectionName)
		{
		}
	}
}
