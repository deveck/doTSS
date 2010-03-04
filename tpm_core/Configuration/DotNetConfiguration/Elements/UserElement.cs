// //
// //
// // Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// // Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration.Elements
{


	/// <summary>
	/// Identifies a single user in the access control list
	/// </summary>
	public class UserElement : ConfigurationElement
	{
		private const string TAG_MEMBERSHIP = "membership";
        private const string TAG_AUTHENTICATIONTYPES = "authenticationTypes";
		
		private const string PROP_UID = "uid";
		private const string PROP_NAME = "name";
		

		[ConfigurationProperty(PROP_UID, IsRequired=true, IsKey=true)]
		public string Uid
		{
			get{ return (string)base[PROP_UID]; }
		}
		
		[ConfigurationProperty(PROP_NAME, IsRequired=true)]
		public string Name
		{
			get{ return (string)base[PROP_NAME]; } 
		}

        [ConfigurationProperty(TAG_AUTHENTICATIONTYPES)]
        public AuthenticationTypesCollection AuthenticationTypes
        {
            get { return (AuthenticationTypesCollection)base[TAG_AUTHENTICATIONTYPES]; }
        }

        [ConfigurationProperty(TAG_MEMBERSHIP)]
        public MembershipCollection Memberships
        {
            get { return (MembershipCollection)base[TAG_MEMBERSHIP]; }
        }
	}


    [ConfigurationCollection(typeof(AuthenticationType), 
        AddItemName="authenticationType", 
        CollectionType=ConfigurationElementCollectionType.BasicMap)]
    public class AuthenticationTypesCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get{ return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "authenticationType"; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AuthenticationType();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as AuthenticationType).Type;
        }
	
    }

    [ConfigurationCollection(typeof(AuthenticationParameter), AddItemName="addAuthParam", CollectionType=ConfigurationElementCollectionType.BasicMap)]
    public class AuthenticationType : ConfigurationElementCollection
    {
        private const string PROP_TYPE = "type";

        [ConfigurationProperty(PROP_TYPE, IsRequired = true)]
        public string Type
        {
            get { return (string)base[PROP_TYPE]; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "addAuthParam"; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AuthenticationParameter();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as AuthenticationParameter).Name;
        }
		
		new public AuthenticationParameter this[string name]
		{
			get{ return (AuthenticationParameter)base[name]; }
		}
    }

    public class AuthenticationParameter : ConfigurationElement
    {
        private const string PROP_NAME = "name";
        private const string PROP_VALUE = "value";

        [ConfigurationProperty(PROP_NAME, IsRequired=true)]
        public string Name
        {
            get { return (string)base[PROP_NAME]; }
        }

        [ConfigurationProperty(PROP_VALUE, IsRequired= true)]
        public string Value
        {
            get { return (string)base[PROP_VALUE]; }
        }
    }

    [ConfigurationCollection(typeof(MembershipElement), AddItemName="memberOf")]
    public class MembershipCollection : ConfigurationElementCollection
    {
        protected override string ElementName
        {
            get { return "memberOf"; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MembershipElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as MembershipElement).Gid;
        }
    }


	/// <summary>
	/// Subelement of the user element which declares the group memberships
	/// of the user
	/// </summary>
	public class MembershipElement : ConfigurationElement
	{
		private const string PROP_GID = "gid";
		
		/// <summary>
		/// Gets the membership group id
		/// </summary>
		[ConfigurationProperty(PROP_GID, IsRequired=true, IsKey=true)]
		public string Gid
		{
			get{  return (string)base[PROP_GID]; }
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
