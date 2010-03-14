
using System;
using System.IO;

namespace Iaik.Utils.Serialization
{

	/// <summary>
	/// Represents a typed primitive-type on the stream
	/// (bool, string, int, byte, byte[])
	/// </summary>
	[TypedStreamSerializable("p")]
	public class TypedPrimitive : ITypedStreamSerializable
	{
		private enum PrimitiveTypeEnum : byte
		{
			Bool = 0,
			String,
			Int,
			Byte,
			ByteA
		}
		
		public delegate void StreamWriteDelegate(object value, Stream sink);
	
		
		private object _value;
		
		public object Value
		{
			get{ return _value;}
		}
		
		public TypedPrimitive (Stream src)
		{
			Read (src);
		}
		
		public TypedPrimitive (object value)
		{
			_value = value;
		}
		
		#region IStreamSerializable implementation
		public void Write (Stream sink)
		{
			if (_value.GetType () == typeof(int))
			{
				sink.WriteByte ((byte)PrimitiveTypeEnum.Int);
				StreamHelper.WriteInt32 ((int)_value, sink);
			}
			else if (_value.GetType () == typeof(bool))
			{
				sink.WriteByte ((byte)PrimitiveTypeEnum.Bool);
				StreamHelper.WriteBool ((bool)_value, sink);
			}
			else if (_value.GetType () == typeof(string))
			{
				sink.WriteByte ((byte)PrimitiveTypeEnum.String);
				StreamHelper.WriteString ((string)_value, sink);
			}
			else if (_value.GetType () == typeof(byte))
			{
				sink.WriteByte ((byte)PrimitiveTypeEnum.Byte);
				sink.WriteByte ((byte)_value);
			}
			else if (_value.GetType () == typeof(byte[]))
			{
				sink.WriteByte ((byte)PrimitiveTypeEnum.ByteA);
				StreamHelper.WriteBytesSafe ((byte[])_value, sink);
			}
			else
				throw new NotSupportedException(string.Format("The type '{0}' is not supported by TypedPrimitive", 
						_value.GetType()));
		}


		public void Read (Stream src)
		{
			PrimitiveTypeEnum primitiveType = (PrimitiveTypeEnum)src.ReadByte ();
			
			
			if (primitiveType == PrimitiveTypeEnum.Int)
				_value = StreamHelper.ReadInt32 (src);
			else if (primitiveType == PrimitiveTypeEnum.Bool)
				_value = StreamHelper.ReadBool (src);
			else if (primitiveType == PrimitiveTypeEnum.String)
				_value = StreamHelper.ReadString (src);
			else if (primitiveType == PrimitiveTypeEnum.Byte)
				_value = src.ReadByte ();
			else if (primitiveType == PrimitiveTypeEnum.ByteA)
				_value = StreamHelper.ReadBytesSafe (src);
			else
				throw new NotSupportedException (string.Format ("The type '{0}' is not supported by TypedPrimitive", primitiveType));
			
		}
		
		#endregion

	}
}