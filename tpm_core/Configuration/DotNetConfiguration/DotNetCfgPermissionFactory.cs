// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;
using System.Configuration;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
{


	public static class DotNetCfgPermissionFactory
	{

		public static PermissionEntry CreatePermissionEntry(AccessControlList acl, string uniquePid, PermissionMember permissionMember)
		{
			if(permissionMember.IdType == IdTypeEnum.User)
			{
				User user = acl.FindUserById(permissionMember.Id);
				if(user == null)
					throw new ConfigurationErrorsException(
					        string.Format("Permission '{0}' contains permission for internal user '{1}' who is not defined", uniquePid, permissionMember.Id));
				return new UserPermissionEntry(permissionMember.Access, user);
			}
			else if(permissionMember.IdType == IdTypeEnum.Group)
			{
				Group group = acl.FindGroupById(permissionMember.Id);
				if(group == null)
					throw new ConfigurationErrorsException(
					        string.Format("Permission '{0}' contains permission for internal group '{1}' which is not defined", uniquePid, permissionMember.Id));
				return new GroupPermissionEntry(permissionMember.Access, group);
			}
			else if(permissionMember.IdType == IdTypeEnum.UserExtern)
				return new ExternUserPermissionEntry(permissionMember.Access, permissionMember.Id);
			else if(permissionMember.IdType == IdTypeEnum.GroupExtern)
				return new ExternGroupPermissionEntry(permissionMember.Access, permissionMember.Id);
			else if(permissionMember.IdType == IdTypeEnum.Meta)
				return new MetaPermissionEntry(permissionMember.Access, permissionMember.Id);
			else
				throw new ConfigurationErrorsException(string.Format("Permission IdType of '{0}' is not supported", permissionMember.IdType));
		}
	}
}
