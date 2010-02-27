// //
// //
// // Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// // Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;

namespace Iaik.Tc.Tpm.Configuration
{


	/// <summary>
	/// Identifies a single user in the access control list
	/// </summary>
	[ConfigurationCollection( typeof(UserMemberShipElement), 
	   CollectionType=ConfigurationElementCollectionType.BasicMap)]
	public class UserElement : ConfigurationElementCollection
	{
		private const string PROP_MEMBEROF_TAG = "memberOf";
		
		
		private const string PROP_UID = "uid";
		private const string PROP_NAME = "name";
		
		#region Static fields
		/// <summary>
		/// Settings for the Uid property
		/// </summary>
		private static ConfigurationProperty _propUid;
		
		/// <summary>
		/// Settings for the Name property
		/// </summary>
		private static ConfigurationProperty _propName;
		
		/// <summary>
		/// Collects all Configuration Properties
		/// </summary>
		private static ConfigurationPropertyCollection _propCollection = new ConfigurationPropertyCollection();
		
		static UserElement()
		{
			_propUid = new ConfigurationProperty(PROP_UID, typeof(string), null, 
			                                   ConfigurationPropertyOptions.IsRequired);
			_propCollection.Add(_propUid);
			_propName = new ConfigurationProperty(PROP_NAME, typeof(string), null, 
			                                   ConfigurationPropertyOptions.IsRequired);
			_propCollection.Add(_propName);
		}
		#endregion

		[ConfigurationProperty(PROP_UID, IsRequired=true, IsKey=true)]
		public string Uid
		{
			get{ return (string)base[_propUid]; }
		}
		
		[ConfigurationProperty(PROP_NAME, IsRequired=true)]
		public string Name
		{
			get{ return (string)base[_propName]; } 
		}

		public override ConfigurationElementCollectionType CollectionType 
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		
		protected override string ElementName 
		{
			get { return PROP_MEMBEROF_TAG; }
		}
		
		protected override ConfigurationElement CreateNewElement ()
		{
			return new UserMemberShipElement();
		}

		protected override object GetElementKey (ConfigurationElement element)
		{
			return (element as UserMemberShipElement).Gid;
		}
		
		protected override ConfigurationPropertyCollection Properties 
		{
			get { return _propCollection; }
		}

	}
	
	/// <summary>
	/// Subelement of the user element which declares the group memberships
	/// of the user
	/// </summary>
	public class UserMemberShipElement : ConfigurationElement
	{
		private const string PROP_GID = "gid";
		
		/// <summary>
		/// Gets the membership group id
		/// </summary>
		[ConfigurationProperty(PROP_GID, IsRequired=true, IsKey=true)]
		public string Gid
		{
			get{ return (string)base[PROP_GID]; }
		}
	}
	
	/// <summary>
	/// Collects all defined users in the access control list
	/// </summary>
	[ConfigurationCollection(typeof(UserElement), 
	                         CollectionType=ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class UserElementCollection : ConfigurationElementCollection
	{
		private const string PROP_USER = "user";

		public override ConfigurationElementCollectionType CollectionType 
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		
		protected override string ElementName 
		{
			get { return PROP_USER; }
		}
		
		protected override ConfigurationElement CreateNewElement ()
		{
			return new UserElement();
		}

		protected override object GetElementKey (ConfigurationElement element)
		{
			return (element as UserElement).Uid;
		}
	}
	
}
