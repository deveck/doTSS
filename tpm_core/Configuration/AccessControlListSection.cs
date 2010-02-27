// //
// //
// // Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// // Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;

namespace Iaik.Tc.Tpm.Configuration
{

	/// <summary>
	/// Parses the access control list from the configuration file
	/// </summary>
	public class AccessControlListSection : ConfigurationSection
	{

		/// <summary>
		/// group collection tag name
		/// </summary>
		private const string PROP_GROUPS = "groups";
		
		/// <summary>
		/// user collection tag name
		/// </summary>
		private const string PROP_USERS = "users";
		
		/// <summary>
		/// permission collection tag name
		/// </summary>
		private const string PROP_PERMISSIONS = "permissions";
	
		/// <summary>
		/// Returns a collection of all defined groups
		/// </summary>
		[ConfigurationProperty(PROP_GROUPS)]
		public GroupElementCollection Groups
		{
			get{ return (GroupElementCollection)base[PROP_GROUPS];}
		}
		
		/// <summary>
		/// Returns a collection of all defined users
		/// </summary>
		[ConfigurationProperty(PROP_USERS)]
		public UserElementCollection Users
		{
			get{ return (UserElementCollection)base[PROP_USERS];}
		}
		
		/// <summary>
		/// Returns a permission collection with associated users
		/// </summary>
		[ConfigurationProperty(PROP_PERMISSIONS)]
		public PermissionsGroup Permissions
		{
			get{ return (PermissionsGroup)base[PROP_PERMISSIONS]; }
		}
	}
}
