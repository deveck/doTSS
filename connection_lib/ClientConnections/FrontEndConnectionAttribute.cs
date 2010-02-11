// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Connection.ClientConnections
{

	/// <summary>
	/// Attribute that is defined for all FrontEndConnection
	/// </summary>
	public class FrontEndConnectionAttribute : Attribute
	{

		/// <summary>
		/// Defines a unique name for the connectiontype thats has this attribute
		/// </summary>
		private string _connectionName;
		
		public string ConnectionName
		{
			get { return _connectionName; }
		}
		
		public FrontEndConnectionAttribute (string connectionName)
		{
			_connectionName = connectionName;
		}
	}
}
