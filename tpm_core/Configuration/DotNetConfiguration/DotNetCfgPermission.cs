// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Configuration.DotNetConfiguration.Elements;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration
{


	public class DotNetCfgPermission : Permission
	{
		private PermissionElement _permissionElement;
		
		public override string Pid 
		{
			get { return _permissionElement.Pid; }
		}
		
		public override string Subsystem 
		{
			get { return _permissionElement.Subsystem; }
		}
		
			
		public DotNetCfgPermission (AccessControlList acl, PermissionElement permissionElement)
		{
			_permissionElement = permissionElement;
			
			foreach(PermissionMember permissionMember in permissionElement)
				_permissionEntries.Add(DotNetCfgPermissionFactory.CreatePermissionEntry(acl, UniqueId, permissionMember));
		}
	}
}
