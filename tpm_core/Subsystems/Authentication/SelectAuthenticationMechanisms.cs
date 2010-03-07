// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.Tpm.Context;
using Iaik.Utils;
using Iaik.Tc.Tpm.Configuration;
using Iaik.Tc.Tpm.Authentication;
using System.Collections.Generic;

namespace Iaik.Tc.Tpm.Subsystems.Authentication
{

	/// <summary>
	/// Requests all available authentication modes for the current connection
	/// from the server
	/// </summary>
    public class SelectAuthenticationMechanismsRequest : TypedSubsystemRequest<SelectAuthenticationMechanismsResponse>
	{

        private string _authMechanismToSelect;

        public string AuthMechanismToSelect
        {
            get { return _authMechanismToSelect; }
            set { _authMechanismToSelect = value; }
        }

		public override string Subsystem 
		{
			get { return SubsystemConstants.SUBSYSTEM_AUTH; }
		}

		public override ushort RequestIdentifier
		{
			get
			{
                return (ushort)AuthenticationSubsystem.AuthenticationRequests.
                    SelectAuthenticationMechanism;
			}
		}


        public SelectAuthenticationMechanismsRequest(EndpointContext ctx)
			:base(ctx)
		{
		}

        public override void Read(Stream src)
        {
            base.Read(src);
            _authMechanismToSelect = StreamHelper.ReadString(src);
        }

        public override void Write(Stream sink)
        {
            base.Write(sink);
            StreamHelper.WriteString(_authMechanismToSelect, sink);
        }
	}
	
	
	/// <summary>
    /// Response to a <see>SelectAuthenticationMechanismsRequest</see>
	/// </summary>
	public class SelectAuthenticationMechanismsResponse : AuthenticationSubsystemResponseBase
	{		
		public SelectAuthenticationMechanismsResponse(SubsystemRequest request, EndpointContext ctx) 
			:base(request, ctx)
		{
		}		
	}
}
