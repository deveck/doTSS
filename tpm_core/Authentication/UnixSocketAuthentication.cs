// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.Tpm.Context;

namespace Iaik.Tc.Tpm.Authentication
{

	/// <summary>
	/// Defines the implicit authentication mechanism for
	/// UnixSocketConnections.
	/// If the supplied connection is not a <see>UnixSocketConnection</see>
	/// an <see>ArgumentException</see> is thrown.
	/// </summary>
	[AuthenticationSettings("unix_auth", typeof(UnixSocketConnection))]
	public sealed class UnixSocketAuthentication : AuthenticationMechanism
	{
		public UnixSocketAuthentication (EndpointContext context)
			:base(context)
		{
			if(typeof(UnixSocketConnection).IsAssignableFrom(context.Connection.GetType()) == false)
				throw new ArgumentException("Supplied connection is not a UnixSocketConnection");
		}
		
		
	}
}
