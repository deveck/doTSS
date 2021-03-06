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


	// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;
using System.Text;
using System.Reflection;

namespace Iaik.Tc.TPM.Library.Common
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


		private static Assembly[] _assemblySearchOrder = null;

		/// <summary>
		/// If typed parameters are defined in multiple assemblies, or in other assemblies than asm(ITypedParameter)
		/// specify them here. They are considered on loading
		/// </summary>
		public static Assembly[] AssemblySearchOrder
		{
			get{ return _assemblySearchOrder; }
			set{ _assemblySearchOrder = value; }
		}

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
			else if (encapsulated_.ContainsKey (key) && typeof(T).IsEnum)
			{
				object value = encapsulated_[key];
				
				if (Enum.GetUnderlyingType (typeof(T)).Equals (value.GetType()))
					return (T)value;
				else
					throw new ArgumentException (string.Format ("Value of type '{0}' is not convertible to enum '{1}'", value.GetType (), typeof(T)));
			}
			else if (encapsulated_.ContainsKey (key))
				return (T)encapsulated_[key];
			else
				throw new KeyNotFoundException (string.Format ("Key '{0}' not found in parameter list", key));
		}
		
		
		public T GetValueOf<T> (String key, T defaultValue)
		{
			try
			{
                if (encapsulated_.ContainsKey(key) == false)
                    return defaultValue;

				return GetValueOf<T> (key);
			}
			catch (KeyNotFoundException)
			{
				return defaultValue;
			}
		}
		
		public bool IsDefined<T>(String key)
		{
			if(encapsulated_.ContainsKey(key) == false)
				return false;
			
			try
			{
				GetValueOf<T>(key);
				return true;
			}
			catch(Exception)
			{
				return false;
			}
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
				
		
				ITypedParameter param;
				
				if(_assemblySearchOrder == null)				 
					param = StreamHelper.ReadTypedStreamSerializable<ITypedParameter> (src);
				else
					param = (ITypedParameter)StreamHelper.ReadTypedStreamSerializable(src, _assemblySearchOrder);
				
				encapsulated_.Add (key, param);
			}
		}
		
		#endregion
		
		public override string ToString ()
		{
			StringBuilder returnStr = new StringBuilder();
			returnStr.AppendFormat("Parameters: count=#{0}\n", this.encapsulated_.Count);
			
			foreach(KeyValuePair<string, ITypedParameter> param in encapsulated_)
			{
				returnStr.AppendFormat("\tname={0}, value={1} \n", param.Key, param.Value);
			}
			
			return returnStr.ToString();
		}

	}
}
