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
	[ConfigurationCollection(typeof(TpmDevice), 
	                         CollectionType=ConfigurationElementCollectionType.BasicMap,
	                         AddItemName="addTpmDevice",
	                         RemoveItemName = "removeTpmDevice",
	                         ClearItemsName = "clearTpmDevices")]
	public class TpmDeviceCollection : ConfigurationElementCollection
	{
	
		public override ConfigurationElementCollectionType CollectionType 
		{
			get { return ConfigurationElementCollectionType.BasicMap;}
		}
		
		protected override string ElementName 
		{
			get { return "addTpmDevice"; }
		}	
		
		protected override ConfigurationElement CreateNewElement ()
		{
			return new TpmDevice();
		}
		
		
		protected override object GetElementKey (ConfigurationElement element)
		{
			return (element as TpmDevice).TpmName;
		}
		
		
	}
}
