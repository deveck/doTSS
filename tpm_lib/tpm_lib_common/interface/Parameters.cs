// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;

namespace Iaik.Tc.Tpm.library.common
{
	/// <summary>
	/// This class represents the parameters list, that is passed from the client to the server
	/// used by TPMCommands
	/// </summary>
	public class Parameters
	{
		/// <summary>
		/// Storage of the parameters list
		/// </summary>
		private IDictionary<String, Object> encapsulated_ = new Dictionary<String, Object>();

		/// <summary>
		/// The standard ctor
		/// </summary>
		public Parameters ()
		{
		}
		
		/// <summary>
		/// Add a value to parameters
		/// </summary>
		/// <param name="key">
		/// A <see cref="String"/>
		/// </param>
		/// <param name="val">
		/// A <see cref="Object"/>
		/// </param>
		public void AddValue(String key, Object val)
		{
			encapsulated_.Add(key, val);
		}
		
		/// <summary>
		/// Get the value of a specified parameter.
		/// </summary>
		/// <param name="key">
		/// A <see cref="String"/>
		/// </param>
		/// <returns>
		/// A <see cref="T"/>
		/// </returns>
		public T GetValueOf<T>(String key)
		{
			return (T) encapsulated_[key];
		}
	}
}
