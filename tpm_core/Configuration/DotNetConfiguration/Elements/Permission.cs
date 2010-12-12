/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;
using System.ComponentModel;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements
{

	/// <summary>
	/// Defines the available permission access modes
	/// </summary>
	public enum AccessEnum
	{
		/// <summary>
		/// Permits access for the associated user/group
		/// </summary>
		Allow,
		
		/// <summary>
		/// Denies access for the associated user/group
		/// </summary>
		Deny
	}
	
	/// <summary>
	/// Defines the available id types
	/// </summary>
	public enum IdTypeEnum
	{
		/// <summary>
		/// Expects an intern (virtual) user-id
		/// </summary>
		User,
		
		/// <summary>
		/// Expects an intern (virtual) group-id
		/// </summary>
		Group,
		
		/// <summary>
		/// Expects an extern (os dependent) user-id
		/// </summary>
		UserExtern,
		
		/// <summary>
		/// expects an extern (os dependent) group-id
		/// </summary>
		GroupExtern,
		
		/// <summary>
		/// Defines some meta ids:
		/// 
		/// all, matches for all users/groups
		/// all_intern, matches for all intern user ids
		/// all_extern, matches for all extern user ids
		/// </summary>
		Meta
	}
	

	/// <summary>
	/// Collects all defined permissions
	/// </summary>
	[ConfigurationCollection(typeof(PermissionElement),
	                         AddItemName="permission",
	                         CollectionType=ConfigurationElementCollectionType.BasicMap
	                         )]
	public class PermissionsGroup : ConfigurationElementCollection
	{

		public override ConfigurationElementCollectionType CollectionType 
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}
		
		protected override string ElementName 
		{
			get { return "permission"; }
		}

		protected override ConfigurationElement CreateNewElement ()
		{
			return new PermissionElement();
		}
		
		protected override object GetElementKey (ConfigurationElement element)
		{
			return string.Format("{0}_{1}", (element as PermissionElement).Subsystem, 
			                     (element as PermissionElement).Pid);
		}
	}
	
	[ConfigurationCollection(typeof(PermissionMember),
	                         CollectionType=ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class PermissionElement : ConfigurationElementCollection
	{
		private const string PROP_SUBSYSTEM = "subsystem";
		private const string PROP_PID = "pid";
		
		
		[ConfigurationProperty(PROP_SUBSYSTEM, IsRequired=true)]
		public string Subsystem
		{
			get{ return (string)base[PROP_SUBSYSTEM]; }
		}
		
		[ConfigurationProperty(PROP_PID, IsRequired=true)]
		public string Pid
		{
			get{ return (string)base[PROP_PID]; }
		}
		
		public override ConfigurationElementCollectionType CollectionType 
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}
		
		protected override ConfigurationElement CreateNewElement ()
		{
			return new PermissionMember();
		}

		protected override object GetElementKey (ConfigurationElement element)
		{
			return string.Format("{0}_{1}_{2}", (element as PermissionMember).IdType, 
			       (element as PermissionMember).Access, (element as PermissionMember).Id);
		}

 
	}
	
	public class PermissionMember:ConfigurationElement
	{
		private const string PROP_ACCESS = "access";
		private const string PROP_IDTYPE = "idType";
		private const string PROP_ID = "id";
		
		[ConfigurationProperty(PROP_ACCESS, IsRequired=true)]
		[TypeConverter(typeof(AccessEnumTypeConverter))]
		public AccessEnum Access
		{
			get{ return (AccessEnum)base[PROP_ACCESS]; }
		}
		
		[ConfigurationProperty(PROP_IDTYPE, IsRequired=true)]
		[TypeConverter(typeof(IdTypeConverter))]
		public IdTypeEnum IdType
		{
			get{ return (IdTypeEnum)base[PROP_IDTYPE]; }
		}
		
		[ConfigurationProperty(PROP_ID, IsRequired=true)]
		public string Id
		{
			get{ return (string)base[PROP_ID]; }
		}
		
		public class IdTypeConverter : ConfigurationConverterBase
		{
			public override object ConvertFrom (System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
			{
				if(value.Equals("group"))
					return IdTypeEnum.Group;
				else if(value.Equals("group_extern"))
					return IdTypeEnum.GroupExtern;
				else if(value.Equals("user"))
					return IdTypeEnum.User;
				else if(value.Equals("user_extern"))
					return IdTypeEnum.UserExtern;
				else if(value.Equals("meta"))
					return IdTypeEnum.Meta;
				else
					throw new ArgumentException(string.Format("'{0}' is not a valid idType!", value));
			}

			public override object ConvertTo (System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
			{
				switch((IdTypeEnum)value)
				{
				case IdTypeEnum.Group:
					return "group";
				case IdTypeEnum.GroupExtern:
					return "group_extern";
				case IdTypeEnum.Meta:
					return "meta";
				case IdTypeEnum.User:
					return "user";
				case IdTypeEnum.UserExtern:
					return "user_extern";
				default:
					throw new ArgumentException(string.Format("Cannot convert IdTypeEnum::'{0}' to string", value));
				}
			}


		}
		
		public class AccessEnumTypeConverter : ConfigurationConverterBase
		{
			public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
			{
				if(value.Equals("allow"))
					return AccessEnum.Allow;
				else if(value.Equals("deny"))
					return AccessEnum.Deny;
				else
					throw new ArgumentException(string.Format("'{0}' is not a valid access mode", value));
			}
			
			public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
			{
				switch((AccessEnum)value)
				{
				case AccessEnum.Allow:
					return "allow";
				case AccessEnum.Deny:
					return "deny";
				default:
					throw new ArgumentException(string.Format("Cannot convert '{0}' to string!", value));
				}
			}

		}
	}
}
