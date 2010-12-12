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


// //
// //
// // Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// // Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements
{
	/// <summary>
	/// Identifies a single group in the access control list
	/// </summary>
	public class GroupElement : ConfigurationElement
	{
		private const string PROP_GID = "gid";
		private const string PROP_NAME = "name";
		

		/// <summary>
		/// Returns the group id of the group (unique)
		/// </summary>
		[ConfigurationProperty(PROP_GID, IsRequired=true, IsKey=true)]
		public string Gid
		{
			get{ return (string)base[PROP_GID]; }
		}
		
		/// <summary>
		/// Returns the name of the group
		/// </summary>
		[ConfigurationProperty(PROP_NAME, IsRequired=true)]
		public string Name
		{
			get{ return (string)base[PROP_NAME]; } 
		}
	}
	
	/// <summary>
	/// Collects all defined groups in the access control list
	/// </summary>
	[ConfigurationCollection(typeof(GroupElement), 
	                         CollectionType=ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class GroupElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Element name of group definitions
		/// </summary>
		private const string PROP_GROUP = "group";
	

		public override ConfigurationElementCollectionType CollectionType 
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		protected override string ElementName 
		{
			get { return PROP_GROUP; }
		}

				
		protected override ConfigurationElement CreateNewElement ()
		{
			return new GroupElement();
		}

		protected override object GetElementKey (ConfigurationElement element)
		{
			return (element as GroupElement).Gid;
		}
	}
}
