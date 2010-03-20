// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Configuration.DotNetConfiguration.Elements;

namespace Iaik.Tc.Tpm.Configuration
{
    /// <summary>
    /// Represents a framework permission
    /// </summary>
    /// <remarks>
    /// <para>
    /// The permission id (<see>Pid</see>) on its own is not unique. 
    /// There could be the same Pid for different subsystems.
    /// A permission entry can only be uniquely identified by providing
    /// the corresponding subsystem AND the Pid
    /// </para>
    /// 
    /// <para>
    /// If nothing is found for a specified user (no corresponding group, no user entry and no meta entry)
    /// the default action applies, which is fixed to deny access at the moment
    /// </para>
	/// </remarks>
    public abstract class Permission
    {
		/// <summary>
		/// Returns the subsystem this permission belongs to
		/// </summary>
		public abstract string Subsystem{get; }
		
		/// <summary>
		/// Returns the permission id of this permission entry.
		/// </summary>
		public abstract string Pid{ get; }
		
		/// <summary>
		/// Lists the permission entries in the same order they are added to the config file
		/// </summary>
		protected List<PermissionEntry> _permissionEntries = new List<PermissionEntry>();
	
		/// <summary>
		/// Checks if the specified permission member is permitted
		/// </summary>
		/// <param name="permissionMember"></param>
		/// <returns></returns>
		public bool IsPermitted(IPermissionMember permissionMember)
		{
			foreach(PermissionEntry entry in _permissionEntries)
			{
				PermissionEntry.PermitEnum permit = entry.Permit(permissionMember);
				
				if(permit != PermissionEntry.PermitEnum.NotFound)
					return permit == PermissionEntry.PermitEnum.Allow? true: false;				   
			}
			
			return false;
		}
		
		
		/// <summary>
		/// Returns if the specified user has permition
		/// </summary>
		/// <param name="user">The user to examine</param>
		/// <returns>
		/// True if user has permission, false otherwise
		/// </returns>
		public bool IsUserPermitted(User user)
		{
			foreach(PermissionEntry entry in _permissionEntries)
			{
				PermissionEntry.PermitEnum permit = entry.PermitUser(user);
				
				if(permit != PermissionEntry.PermitEnum.NotFound)
					return permit == PermissionEntry.PermitEnum.Allow? true: false;				   
			}
			
			return false;
		}
		
		/// <summary>
		/// Returns if the specified external user has permition
		/// </summary>
		/// <param name="user">The user to examine</param>
		/// <returns>
		/// True if the user has permission, false otherwise
		/// </returns>
		public bool IsExternalUserPermitted(ExternalUser user)
		{
			foreach(PermissionEntry entry in _permissionEntries)
			{
				PermissionEntry.PermitEnum permit = entry.PermitUser(user);
				
				if(permit != PermissionEntry.PermitEnum.NotFound)
					return permit == PermissionEntry.PermitEnum.Allow? true: false;				   
			}
			
			return false;
		}
				
		
		public override bool Equals (object obj)
		{
			if(obj is Permission)
			{
				return (obj as Permission).Subsystem.Equals(Subsystem) 
					&& (obj as Permission).Pid.Equals(Pid);				        
			}
			
			return false;
		}
		
		public override int GetHashCode ()
		{
			return Subsystem.GetHashCode() ^ Pid.GetHashCode();
		}


		public string UniqueId
		{
			get{ return BuildUniquePermissionId(Subsystem, Pid);}
		}
		
		public static string BuildUniquePermissionId(string subsystem, string pid)
		{
			return string.Format("{0}_{1}", subsystem, pid);
		}
    }
	
	public abstract class PermissionEntry
	{
		
		public enum PermitEnum
		{
			/// <summary>
			/// Tells the caller that a user/group permission was found and
			/// access should be exclusivly allowed
			/// </summary>
			Allow,
			
			/// <summary>
			/// Thells the caller that a user/group permission was found and
			/// access should be exclusivly denied
			/// </summary>
			Deny,
			
			/// <summary>
			/// Tells the caller that no user/group permission was found
			/// </summary>
			NotFound
		}
		
		protected AccessEnum _access;
		
		/// <summary>
		/// Returns the access mode (deny, allow) for this permission
		/// </summary>
		public AccessEnum Access
		{
			get{ return _access; }
		}
		
		protected abstract IdTypeEnum IdType { get; }
		
		protected abstract string Id {get;}
		
		public PermissionEntry(AccessEnum access)
		{
			_access = access;
		}
		
		protected virtual bool IsUser(User user)
		{
			return false;
		}
		
		protected virtual bool IsUser(ExternalUser user)
		{
			return false;
		}
		
		public virtual PermitEnum PermitUser(User user)
		{
			if(IsUser(user))
				return _access == AccessEnum.Allow?PermitEnum.Allow:PermitEnum.Deny;
			else
				return PermitEnum.NotFound;
				
		}
		
		public virtual PermitEnum PermitUser(ExternalUser user)
		{
			if(IsUser(user))
				return _access == AccessEnum.Allow?PermitEnum.Allow:PermitEnum.Deny;
			else
				return PermitEnum.NotFound;
		}
		
		public virtual PermitEnum Permit(IPermissionMember permissionMember)
		{
			if(IdType == permissionMember.IdType && Id == permissionMember.Id)
				return _access == AccessEnum.Allow?PermitEnum.Allow:PermitEnum.Deny;
			else
			{
				foreach(IPermissionMember subMember in permissionMember.SubPermissionMembers)
				{
					PermitEnum permit = Permit(subMember);
					
					if(permit != PermitEnum.NotFound)
						return permit;
				}
			}
			
			return PermitEnum.NotFound;
		}
	}
	
	public class UserPermissionEntry : PermissionEntry
	{
		protected User _user;
		
		public User User
		{
			get{ return _user;}
		}
		
		protected override IdTypeEnum IdType
		{
			get{ return IdTypeEnum.User; }
		}
		
		protected override string Id 
		{
			get { return _user.Uid; }
		}

		
		public UserPermissionEntry(AccessEnum access, User user)
			:base(access)
		{
			_user = user;
		}
		
		protected override bool IsUser (User user)
		{
			return _user.Equals(user);
		}

	}
	
	public class GroupPermissionEntry : PermissionEntry
	{
		protected Group _group;
		
		public Group Group
		{
			get{ return _group; }
		}
		
		protected override IdTypeEnum IdType
		{
			get{ return IdTypeEnum.Group; }
		}
		
		protected override string Id 
		{
			get { return _group.Gid; }
		}

				
		public GroupPermissionEntry(AccessEnum access, Group group)
			:base( access)
		{
			_group = group;
		}
		
		protected override bool IsUser (User user)
		{
			return user.IsMemberOf(_group);
		}

	}
	
	public class ExternUserPermissionEntry : PermissionEntry	
	{
		protected string _externalUserId;
		
		public string ExternalUserId
		{
			get{ return _externalUserId; }
		}
		
		protected override IdTypeEnum IdType
		{
			get{ return IdTypeEnum.UserExtern; }
		}
		
		protected override string Id 
		{
			get { return _externalUserId; }
		}

				
		public ExternUserPermissionEntry(AccessEnum access, string externalUserId)
			:base (access)
		{
			_externalUserId = externalUserId;
		}
			
		
		protected override bool IsUser (ExternalUser user)
		{
			return _externalUserId.Equals(user.UId);
		}

	}
	
	public class ExternGroupPermissionEntry : PermissionEntry
	{
		protected string _externalGroupId;
		
		public string ExternalGroupId
		{
			get{ return _externalGroupId; }
		}
		
		protected override IdTypeEnum IdType
		{
			get{ return IdTypeEnum.GroupExtern; }
		}
		
		protected override string Id 
		{
			get { return _externalGroupId; }
		}

			
		public ExternGroupPermissionEntry(AccessEnum access, string externalGroupId)
			:base(access)
		{
			_externalGroupId = externalGroupId;
		}
		
		protected override bool IsUser (ExternalUser user)
		{
			return _externalGroupId.Equals(user.Group.Id);
		}

	}
	
	public class MetaPermissionEntry : PermissionEntry
	{
		private const string ID_ALL = "all";
		private const string ID_INTERNAL = "all_internal";
		private const string ID_EXTERNAL = "all_external";
		
		private string _id;
		
		public string MetaId
		{
			get{ return Id;}
		}
		
		/// <summary>
		/// Identifier of the meta users to apply to
		/// </summary>
		protected override string Id
		{
			get{ return _id;}
		}
		
		protected override IdTypeEnum IdType
		{
			get{ return IdTypeEnum.Meta; }
		}
		
		
		public MetaPermissionEntry(AccessEnum access, string id)
			:base(access)
		{
			_id = id;
		}
		
		protected override bool IsUser (ExternalUser user)
		{
			return _id.Equals(ID_ALL) || _id.Equals(ID_EXTERNAL);	
		}
		
		protected override bool IsUser (User user)
		{
			return _id.Equals (ID_ALL) || _id.Equals (ID_INTERNAL);
		}
		
		public override PermissionEntry.PermitEnum Permit (IPermissionMember permissionMember)
		{
			if (_id.Equals (ID_ALL))
				return _access == AccessEnum.Allow ? PermitEnum.Allow : PermitEnum.Deny;
			
			return base.Permit (permissionMember);
			
		}

	}
}
