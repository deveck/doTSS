// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Configuration.DotNetConfiguration.Elements;

namespace Iaik.Tc.Tpm.Configuration
{
	public class ExternalGroup : IPermissionMember
	{
		private string _gid;
		
		public ExternalGroup (string gid)
		{
			_gid = gid;
		}
		
		#region IPermissionMember implementation
		public IdTypeEnum IdType 
		{
			get { return IdTypeEnum.GroupExtern; }
		}
		
		
		public string Id 
		{
			get { return _gid; }
		}
		
		
		public System.Collections.Generic.IEnumerable<IPermissionMember> SubPermissionMembers 
		{
			get { return new IPermissionMember[0]; }
		}
		
		#endregion

		public ExternalGroup ()
		{
		}
	}
}
