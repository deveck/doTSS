
using System;
using System.Reflection;

namespace Iaik.Utils.Hash
{

	/// <summary>
	/// Provides data for hasher for enum types
	/// </summary>
	public class HashEnumDataProvider : HashDataProvider
	{
		
		/// <summary>
		/// The EnumDataProvider is just a wrapper around a primitive data provider
		/// </summary>
		private HashPrimitiveDataProvider _subDataProvider;
		
		
		
		public HashEnumDataProvider (object value)
		{
			Type t = value.GetType ();
			
			if (!t.IsEnum)
				throw new ArgumentException ("HashEnumDataProvider only support enum types");
			
			ConstructorInfo ctorInfo = typeof(HashPrimitiveDataProvider).GetConstructor (new Type[] { t.UnderlyingSystemType });
			
			if (ctorInfo == null)
				throw new ArgumentException ("Cannot find HashPrimitiveDataProvider with appropriate ctor");
			
			_subDataProvider = (HashPrimitiveDataProvider)ctorInfo.Invoke (new object[] { value });
		}
		
		
		
		public override int NextBytes (byte[] buffer)
		{
			return _subDataProvider.NextBytes (buffer);
		}

	}
}
