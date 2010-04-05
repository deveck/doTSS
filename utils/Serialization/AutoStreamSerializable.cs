
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace Iaik.Utils.Serialization
{

	/// <summary>
	/// Finally implemented by classes which support automatic stream (reflective supported) serialization.
	/// All members to serialize have attached an SerializeMeAttribute
	/// </summary>
	public abstract class AutoStreamSerializable : IStreamSerializable
	{
		
		/// <summary>
		/// Returns the member information for all Members to de-/serialize
		/// </summary>
		/// <returns>
		/// A <see cref="FieldInfo[]"/>
		/// </returns>
		private FieldInfo[] GetMembersToSerializeInOrder ()
		{
			IDictionary<int, FieldInfo> dictMemberInfos = new SortedDictionary<int, FieldInfo> ();
			
			foreach (FieldInfo memberInfo in this.GetType ().GetFields (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				object[] attachedAttributes = memberInfo.GetCustomAttributes (typeof(SerializeMeAttribute), false);
			
				if (attachedAttributes != null && attachedAttributes.Length > 0)
					dictMemberInfos.Add (((SerializeMeAttribute)attachedAttributes[0]).Ordinal, memberInfo);
			}
			
			
			List<FieldInfo> memberInfos = new List<FieldInfo> ();
			
			foreach (FieldInfo memberInfo in dictMemberInfos.Values)
				memberInfos.Add (memberInfo);
			
			return memberInfos.ToArray ();
		}
		
		
		#region IStreamSerializable implementation
		public virtual void Write (Stream sink)
		{
			foreach (FieldInfo memberInfo in GetMembersToSerializeInOrder ())
			{
				Type curType = memberInfo.FieldType;
				
				if (typeof(IStreamSerializable).IsAssignableFrom (curType))
				{
					IStreamSerializable toSerialize = (IStreamSerializable)memberInfo.GetValue (this);
					
					if (toSerialize == null)
						StreamHelper.WriteBool (false);
					else
					{
						StreamHelper.WriteBool (true);
						toSerialize.Write (sink);						
					}
				}
				else if (curType == typeof(byte))
					sink.WriteByte ((byte)memberInfo.GetValue (this));
				else if (curType == typeof(byte[]))
					StreamHelper.WriteBytesSafe ((byte[])memberInfo.GetValue (this), sink);
				else if (curType == typeof(int))
					StreamHelper.WriteInt32 ((int)memberInfo.GetValue (this), sink);
				else if (curType == typeof(ushort))
					StreamHelper.WriteUInt16 ((ushort)memberInfo.GetValue (this), sink);
				else
					throw new ArgumentException (string.Format ("Type '{0}' is not supported by AutoStreamSerializable", curType));
			}
		}


		public virtual void Read (Stream src)
		{
			foreach (FieldInfo memberInfo in GetMembersToSerializeInOrder ())
			{
				Type curType = memberInfo.FieldType;
				
				if (typeof(IStreamSerializable).IsAssignableFrom (curType))
				{
					ConstructorInfo ctorInfo = curType.GetConstructor (new Type[] {  });
					
					if (ctorInfo == null)
						throw new ArgumentException (string.Format ("'{0}' is not compatible with AutoStreamSerializable, no ctor(Stream) found!", curType));
					
					bool hasValue = StreamHelper.ReadBool (src);
					
					if (hasValue)
					{
						memberInfo.SetValue (this, ctorInfo.Invoke (new object[] {  }));
						((IStreamSerializable)memberInfo.GetValue (this)).Read (src);
					}
					else
						memberInfo.SetValue (this, null);
					
				}
				else if (curType == typeof(byte))
					memberInfo.SetValue (this, (byte)src.ReadByte ());
				else if (curType == typeof(byte[]))
					memberInfo.SetValue (this, StreamHelper.ReadBytesSafe (src));
				else if (curType == typeof(int))
					memberInfo.SetValue (this, StreamHelper.ReadInt32 (src)); 
				else if (curType == typeof(ushort))
					memberInfo.SetValue (this, StreamHelper.ReadUInt16 (src));
				else
					throw new ArgumentException (string.Format ("Type '{0}' is not supported by AutoStreamSerializable", curType));
			}
		}
		
		#endregion
	}
}
