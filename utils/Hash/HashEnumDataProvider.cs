
using System;
using System.Reflection;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Utils.Hash
{

	/// <summary>
	/// Provides data for hasher for enum types
	/// </summary>
	[TypedStreamSerializable("h_enum_dp")]
	public class HashEnumDataProvider : HashDataProvider
	{
		
		/// <summary>
		/// The EnumDataProvider is just a wrapper around a primitive data provider
		/// </summary>
		[SerializeMe(0)]
		private HashPrimitiveDataProvider _subDataProvider;
		
		
		
		public HashEnumDataProvider (object value)
		{
			Type t = value.GetType ();
			
			if (!t.IsEnum)
				throw new ArgumentException ("HashEnumDataProvider only support enum types");

			Type baseType = Enum.GetUnderlyingType(t);
			
			ConstructorInfo ctorInfo = typeof(HashPrimitiveDataProvider).GetConstructor (new Type[] { baseType });
			
			if (ctorInfo == null)
				throw new ArgumentException ("Cannot find HashPrimitiveDataProvider with appropriate ctor");
			
			if(baseType == typeof(uint))
				_subDataProvider = (HashPrimitiveDataProvider)ctorInfo.Invoke (new object[] { (uint)value });
			else if(baseType == typeof(ushort))
				_subDataProvider = (HashPrimitiveDataProvider)ctorInfo.Invoke (new object[] { (ushort)value });
			else if(baseType == typeof(byte))
				_subDataProvider = (HashPrimitiveDataProvider)ctorInfo.Invoke (new object[] { (byte)value });
			else
				throw new ArgumentException("Type not supported");
		}
		
		
		public HashEnumDataProvider(Stream src)
		{
			Read(src);
		}
		
		public override int NextBytes (byte[] buffer)
		{
			return _subDataProvider.NextBytes (buffer);
		}

	}
}
