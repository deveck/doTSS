// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Configuration;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration
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
