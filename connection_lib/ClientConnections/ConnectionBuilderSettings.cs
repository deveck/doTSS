// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Connection.ClientConnections
{

	/// <summary>
	/// All settings required by the connection builders
	/// </summary>
	public class ConnectionBuilderSettings
	{

		private RequestSecretDelegate _requestSecret = null;
		
		/// <summary>
		/// Used to request a protected secret from the user
		/// </summary>
		public RequestSecretDelegate RequestSecret 
		{
			get { return _requestSecret; }
			set { _requestSecret = value;}
		}

		public ConnectionBuilderSettings ()
		{
		}
		
		public ConnectionBuilderSettings(RequestSecretDelegate requestSecret)
		{
			_requestSecret = requestSecret;
		}
	}
}
