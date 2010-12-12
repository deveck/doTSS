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

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
{

	/// <summary>
	/// Collects all configured listeners
	/// </summary>
	[ConfigurationCollection(typeof(Listener), 
	                         CollectionType=ConfigurationElementCollectionType.BasicMap,
	                         AddItemName="addListener",
	                         RemoveItemName = "removeListener",
	                         ClearItemsName = "clearListeners")]
	public class ListenersCollection : ConfigurationElementCollection
	{
	
		public override ConfigurationElementCollectionType CollectionType 
		{
			get { return ConfigurationElementCollectionType.BasicMap;}
		}
		
		protected override string ElementName 
		{
			get { return "addListener"; }
		}	
		
		protected override ConfigurationElement CreateNewElement ()
		{
			return new Listener();
		}
		
		
		protected override object GetElementKey (ConfigurationElement element)
		{
			return (element as Listener).ListenerType;
		}
		
		
	}
}
