// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Subsystems.Authentication;
using Mono.Unix;
using Iaik.Tc.TPM.Configuration;
using Iaik.Tc.TPM.Subsystems;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;
using System.Security.Cryptography.X509Certificates;
using log4net;

namespace Iaik.Tc.TPM.Authentication
{

	/// <summary>
	/// Defines an authentication mechanism where the users are authenticated based on their
	/// certificate hash
	/// </summary>
	[AuthenticationSettings("ssl_auth", typeof(SslConnection))]
	public class SslAuthentication : AuthenticationMechanism
	{
		private ILog _log = LogManager.GetLogger("SslAuthentication");
		
		protected ServerContext ServerContext
		{
			get{ return (ServerContext)_context;}
		}
		
		public SslAuthentication(IAuthenticationMethod authConfig)
		{			
		}
		
        public override void Initialize(EndpointContext context)
        {
			if(!(context is ServerContext))
				throw new ArgumentException("SslAuthentication expects ServerContext");
			
            base.Initialize(context);
        }
		
		public override void Authenticate (RequestContext<AuthenticateRequest, AuthenticateResponse> requestContext)
		{
			SslConnection connection = _context.Connection as SslConnection;
			
			X509Certificate cert = connection.ClientCertificate;
			string certHashString = cert.GetCertHashString();
			
			foreach(User user in ServerContext.AccessControlList.Users)
			{
				SslAuthenticationParameters auth = (SslAuthenticationParameters)user.GetAuthentication("ssl_auth");

				
				if(auth != null && auth.CertificateHash.Equals(certHashString))
				{
					ServerContext.ServerAuthenticationContext.AuthenticatedPermissionMember = user;
				
					AuthenticateResponse response = requestContext.CreateResponse();
					response.Succeeded = true;
					response.Execute();
					return;
				}
			}
	
			_log.WarnFormat("Could not find user associated with certificate '{0}'", certHashString);
		
			AuthenticateResponse errorResponse = requestContext.CreateResponse();
			errorResponse.Succeeded = false;
			errorResponse.CustomErrorMessage = "No client associated with specified certificate!";
			errorResponse.Execute();				
	
		}

		
	}
}
