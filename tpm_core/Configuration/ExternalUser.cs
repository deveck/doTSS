// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Configuration
{

	/// <summary>
	/// Represents an external (os dependent) user of the framework
	/// </summary>
	public class ExternalUser : IPermissionMember
	{
		/// <summary>
		/// User identifier
		/// </summary>
		protected string _userId;
		
		/// <summary>
		/// Gets the user id of the external user
		/// </summary>
		public string UId
		{
			get{ return _userId;}
		}
		
		/// <summary>
		/// Group identifier
		/// </summary>
		protected ExternalGroup _group;
		
		/// <summary>
		/// Gets the group id of the external user
		/// </summary>
		public ExternalGroup Group
		{
			get{ return _group;}
		}
		
		
		public ExternalUser (string uid, string gid)
		{
			_userId = uid;
			_group = new ExternalGroup(gid);
		}
		
			
		public IdTypeEnum IdType
		{
			get{ return IdTypeEnum.UserExtern; }
		}
		
		public string Id
		{
			get{ return UId; }
		}
		
		public IEnumerable<IPermissionMember> SubPermissionMembers
		{
			get{ return new IPermissionMember[]{_group}; }
		}
		
	}
}
