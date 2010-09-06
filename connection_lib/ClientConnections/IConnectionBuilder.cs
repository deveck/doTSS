// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Hash;

namespace Iaik.Connection.ClientConnections
{
	/// <summary>
	/// Requests a protected secret from the client
	/// </summary>
	public delegate ProtectedPasswordStorage RequestSecretDelegate(string userHint);
	
	/// <summary>
	/// Some connection types may require user interaction
	/// (e.g. ssl connection requires the user to enter
	/// the certificate password). If a FrontEndConnectionAttribute
	/// is associated with a connection builder, the connection builder
	/// is used to setup the connection
	/// </summary>
	public interface IConnectionBuilder
	{

		/// <summary>
		/// Settings required by the connection builder
		/// </summary>
		ConnectionBuilderSettings Settings{get; set;}
		
		/// <summary>
		/// Creates the associated Frontend connection
		/// </summary>
		/// <returns></returns>
		FrontEndConnection SetupConnection();
	}
}
