using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Configuration.DotNetConfiguration.Elements;
using System.Configuration;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration
{
    /// <summary>
    /// Dot Net Configuration implementation of the user class
    /// </summary>
    public class DotNetCfgUser : User
    {
        /// <summary>
        /// the UserElement from the parsed configuration
        /// </summary>
        private UserElement _userElement;

        public DotNetCfgUser(AccessControlList acl, UserElement userElement)
        {
            _userElement = userElement;
			_memberOf = new Dictionary<string, Group>();
			
			foreach(MembershipElement memberOf in userElement.Memberships)
			{
				if(_memberOf.ContainsKey(memberOf.Gid))
					throw new ConfigurationErrorsException(string.Format("Group membership '{0}' is specified more than once for user '{1}'", memberOf.Gid, Uid));
				
				
				Group myGroup = acl.FindGroupById(memberOf.Gid);
				
				if(myGroup == null)
					throw new ConfigurationErrorsException(
					       string.Format("User '{0}'  defines membership to group '{1}' but group does not exist",
					                      Uid, memberOf.Gid));
				
				_memberOf.Add(myGroup.Gid, myGroup);
			}
			
			
			_authentications = new Dictionary<string, Authentication>();
			
			foreach(AuthenticationType authenticationType in userElement.AuthenticationTypes)
			{
				Authentication auth = DotNetCfgAuthenticationFactory.CreateAuthentication(authenticationType);
				
				if(auth != null)
					_authentications.Add(auth.AuthenticationType, auth);
				else
					throw new ConfigurationErrorsException(
					        string.Format("User '{0}' want to use authentication type '{1}', but this type is not known!", 
					                       Uid, authenticationType.Type));
			}
        }

        public override string Uid
        {
            get { return _userElement.Uid; }
        }

        public override string Name
        {
            get { return _userElement.Name; }
        }
    }
}
