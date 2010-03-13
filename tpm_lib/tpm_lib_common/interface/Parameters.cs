// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.Tpm.library.common
{
	/// <summary>
	/// This class represents the parameters list, that is passed from the client to the server
	/// used by TPMCommands
	/// </summary>
	public class Parameters : IStreamSerializable
	{
		/// <summary>
		/// Storage of the parameters list
		/// </summary>
		private IDictionary<String, ITypedParameter> encapsulated_ = 
			new Dictionary<String, ITypedParameter>();

		/// <summary>
		/// The standard ctor
		/// </summary>
		public Parameters ()
		{
		}
		
		public Parameters (Stream src)
		{
			Read (src);
		}
		
		/// <summary>
		/// Add a value to parameters
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		public void AddValue (String key, ITypedParameter val)
		{
			encapsulated_.Add (key, val);
		}
		
		public void AddPrimitiveType (String key, object value)
		{
			encapsulated_.Add (key, new TypedPrimitiveParameter (value));
		}
		
		/// <summary>
		/// Get the value of a specified parameter.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public T GetValueOf<T> (String key)
		{
			if (encapsulated_.ContainsKey (key) && 
				encapsulated_[key] is TypedPrimitiveParameter)
				return (T)((TypedPrimitiveParameter)encapsulated_[key]).Value;
			else 
				return (T)encapsulated_[key];
		}
		
		
		#region IStreamSerializable implementation
		public void Write (Stream sink)
		{
			StreamHelper.WriteInt32 (encapsulated_.Count, sink);
			foreach (KeyValuePair<string, ITypedParameter> parameterValue in encapsulated_)
			{
				StreamHelper.WriteString (parameterValue.Key, sink);
				StreamHelper.WriteTypedStreamSerializable (parameterValue.Value, sink);
			}
		}
		
		
		public void Read (Stream src)
		{
			int count = StreamHelper.ReadInt32 (src);
			
			for (int i = 0; i < count; i++)
			{
				string key = StreamHelper.ReadString (src);
				ITypedParameter param = StreamHelper.ReadTypedStreamSerializable<ITypedParameter> (src);
				encapsulated_.Add (key, param);
			}
		}
		
		#endregion
	}
}
