// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.Tpm.Configuration
{

	/// <summary>
	/// Represents an external (os dependent) user of the framework
	/// </summary>
	public class ExternalUser
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
		protected string _groupId;
		
		/// <summary>
		/// Gets the group id of the external user
		/// </summary>
		public string GId
		{
			get{ return _groupId;}
		}
		
		
		public ExternalUser (string uid, string gid)
		{
			_userId = uid;
			_groupId = gid;
		}
	}
}
