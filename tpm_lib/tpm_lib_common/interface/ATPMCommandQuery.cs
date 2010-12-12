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



using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.Common
{


	public abstract class ATPMCommandQuery : IStreamSerializable
	{
		/// <summary>
		/// The TPM Command identifier for which this query belongs to
		/// </summary>
		private String _commandIdentifier;
		
		/// <summary>
		/// The Parameters for this specific query
		/// </summary>
		private Parameters _params;
		
		/// <summary>
		/// The standard ctor of any query, it is private because we won't use uninitialized querys
		/// </summary>
		protected ATPMCommandQuery ()
		{
		}
		
		/// <summary>
		/// Ctor that initialises query from serialized form
		/// </summary>
		/// <param name="src">
		/// A <see cref="Stream"/>
		/// </param>
		public ATPMCommandQuery (Stream src)
		{
			Read (src);
		}
		
		/// <summary>
		/// Ctor that initialises query with commandIdentifier and parameters
		/// </summary>
		/// <param name="commandIdentifier">
		/// A <see cref="String"/>
		/// </param>
		/// <param name="param">
		/// A <see cref="Parameters"/>
		/// </param>
		public ATPMCommandQuery(String commandIdentifier, Parameters param)
		{
			_commandIdentifier = commandIdentifier;
			_params = param;
		}
		
		/// <summary>
		/// Get the parameters
		/// </summary>
		public Parameters Parameters
		{
			get
			{
				return _params; 
			}
			
			set
			{
				if (value == null)
					throw new ArgumentNullException ("params_", "TPMCommandRequest params_ must not be null");
				_params = value;
			}
		}
		
		/// <summary>
		/// Get the commandIdentifier
		/// </summary>
		public string CommandIdentifier
		{
			get { return _commandIdentifier;}
		}
		
		#region IStreamSerializable implementation
		/// <summary>
		/// Write query to stream
		/// </summary>
		/// <param name="sink">
		/// A <see cref="Stream"/>
		/// </param>
		public virtual void Write (Stream sink)
		{
			StreamHelper.WriteString (_commandIdentifier, sink);
			_params.Write (sink);
		}
		
		/// <summary>
		/// Read query from stream
		/// </summary>
		/// <param name="src">
		/// A <see cref="Stream"/>
		/// </param>
		public virtual void Read (Stream src)
		{
			_commandIdentifier = StreamHelper.ReadString (src);
			_params = new Parameters (src);
		}
		#endregion
	}
}
