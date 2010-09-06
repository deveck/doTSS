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
		private Type _connectionBuilder = null;
		
		/// <summary>
		/// If the attribute has an associated connection builder,
		/// use it to build connection types with a more complex setup process
		/// </summary>
		public Type ConnectionBuilder
		{
			get{ return _connectionBuilder;}
		}
		
		
		public FrontEndConnectionAttribute (string connectionName)
			:base(connectionName)
		{
		}
		
		public FrontEndConnectionAttribute(string connectionName, Type connectionBuilder)
			:this(connectionName)
		{
			_connectionBuilder = connectionBuilder;
		}
		
	}
}
