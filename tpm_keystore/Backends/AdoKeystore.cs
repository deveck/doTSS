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
using System.Data;

namespace Iaik.Tc.TPM.Keystore.Backends
{

	/// <summary>
	/// Provides a keystore base implementation for ado .net data providers
	/// </summary>
	public abstract class AdoKeystore : TPMKeystoreProvider
	{
		/// <summary>
		/// Connection to the database
		/// </summary>
		protected IDbConnection _connection = null;


		public AdoKeystore (IDictionary<string, string> parameters)
		{
			BuildConnection(parameters);
		}
		
		/// <summary>
		/// Builds the connection to the database (_connection)
		/// </summary>
		/// <param name="parameters">Parameters as provided in the configuration</param>
		protected abstract void BuildConnection(IDictionary<string, string> parameters);
		
		
		/// <summary>
		/// Builds a db command
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected virtual IDbCommand BuildCommand(string query)
		{
			IDbCommand cmd = _connection.CreateCommand();
			cmd.CommandText = query;
			return cmd;
		}
		
		public override Int64 KeyCount 
		{
			get 
			{
				using(IDbCommand cmd = BuildCommand("SELECT COUNT(*) FROM tpm_keys ;"))
				{
					object i = cmd.ExecuteScalar();
					return (Int64)i;
				}
			}
		}

		
		public override string[] EnumerateFriendlyNames ()
		{
			using(IDbCommand cmd = BuildCommand("SELECT friendly_name FROM tpm_keys ORDER BY friendly_name"))
			{
				return ReadStringColumn(cmd);
			}
		}

		public override string[] EnumerateIdentifiers ()
		{
			using(IDbCommand cmd = BuildCommand("SELECT identifier FROM tpm_keys ORDER BY identifier"))
			{
				return ReadStringColumn(cmd);
			}
		}
		
		public override Nullable<KeyValuePair<string, string>> FindParentKeyByFriendlyName (string friendlyName)
		{
			using(IDbCommand cmd = BuildCommand(string.Format(@"
					SELECT pk.friendly_name, pk.identifier FROM tpm_keys 
					LEFT JOIN tpm_keys AS pk ON tpm_keys.parent_key= pk.friendly_name
					WHERE tpm_keys.friendly_name={0}
					ORDER BY pk.friendly_name", DeriveParameterName("friendlyName"))))
			{
				CreateParameter(cmd, "friendlyName", DbType.String, friendlyName);
		
				return ReadKeyIdentifiers(cmd);
			}
		}

		public override Nullable<KeyValuePair<string, string>> FindParentKeyByIdentifier (string identifier)
		{
			using(IDbCommand cmd = BuildCommand(string.Format(@"
					SELECT pk.friendly_name, pk.identifier FROM tpm_keys 
					LEFT JOIN tpm_keys AS pk ON tpm_keys.parent_key= pk.friendly_name
					WHERE tpm_keys.identifier={0}
					ORDER BY pk.friendly_name", DeriveParameterName("identifier"))))
			{
				CreateParameter(cmd, "identifier", DbType.String, identifier);
				
				return ReadKeyIdentifiers(cmd);
			}
		}
					
		public override string FriendlyNameToIdentifier (string friendlyName)
		{
			using(IDbCommand cmd = BuildCommand(string.Format(@"
					SELECT identifier FROM tpm_keys 
					WHERE friendly_name={0}", DeriveParameterName("friendlyName"))))
			{
				CreateParameter(cmd, "friendlyName", DbType.String, friendlyName);
		
				object identifier = cmd.ExecuteScalar();
			
				if(identifier == null || identifier == DBNull.Value)
					return null;
				else
					return (string)identifier;
			}
		}
		
		public override string IdentifierToFriendlyName (string identifier)
		{
			using(IDbCommand cmd = BuildCommand(string.Format(@"
					SELECT friendly_name FROM tpm_keys 
					WHERE identifier={0}", DeriveParameterName("identifier"))))
			{
				CreateParameter(cmd, "identifier", DbType.String, identifier);
				
				object friendlyName = cmd.ExecuteScalar();
			
				if(friendlyName == null || friendlyName == DBNull.Value)
					return null;
				else
					return (string)friendlyName;
			}
		}
		
		public override byte[] GetKeyBlob (string identifier)
		{
			using(IDbCommand cmd = BuildCommand(string.Format(@"
					SELECT key_data FROM tpm_keys 
					WHERE identifier={0}", DeriveParameterName("identifier"))))
			{
				CreateParameter(cmd, "identifier", DbType.String, identifier);
			
				object keyData = cmd.ExecuteScalar();
			
				if(keyData == null || keyData == DBNull.Value)
					return null;
				else
					return (byte[])keyData;
			}
		}


		public override void AddKey (string friendlyName, string identifier, string parentFriendlyName, byte[] keyData)
		{
			using(IDbCommand cmd = BuildCommand(string.Format(@"
				INSERT INTO tpm_keys (friendly_name, identifier, creation_date, parent_key, key_data)
				VALUES ({0}, {1}, {2}, {3}, {4})",
				DeriveParameterName("friendlyName"), 
				DeriveParameterName("identifier"), 
				DeriveParameterName("creationDate"), 
				DeriveParameterName("parentKey"),
				DeriveParameterName("keyData"))))
			{

				CreateParameter(cmd, "friendlyName", DbType.String, friendlyName);
				CreateParameter(cmd, "identifier", DbType.String, identifier);
				CreateParameter(cmd, "creationDate", DbType.DateTime, DateTime.Now);
				CreateParameter(cmd, "parentKey", DbType.String, parentFriendlyName);
				CreateParameter(cmd, "keyData", DbType.Binary, keyData);

				cmd.ExecuteNonQuery();
			}
		}


		public override void RemoveKey (string friendlyName)
		{
			using(IDbCommand cmd = BuildCommand(string.Format(@"
				DELETE FROM tpm_keys WHERE friendly_name={0}", DeriveParameterName("friendlyName"))))
			{
				CreateParameter(cmd, "friendlyName", DbType.String, friendlyName);
				cmd.ExecuteNonQuery();
			}
		}
		
		public override bool ContainsIdentifier (string identifier)
		{
			using(IDbCommand cmd = BuildCommand(string.Format(@"
				SELECT COUNT(*) FROM tpm_keys WHERE identifier={0}", DeriveParameterName("identifier"))))
			{
				CreateParameter(cmd, "identifier", DbType.String, identifier);
				return ((Int64)cmd.ExecuteScalar()) > 0;
			}
		}


								
		private string[] ReadStringColumn(IDbCommand cmd)
		{
			List<string> values = new List<string>();
			
			using(IDataReader rdr = cmd.ExecuteReader())
			{
				while(rdr.Read())
					values.Add(rdr.GetString(0));
			}
			
			return values.ToArray();
		}
		
		private KeyValuePair<string, string>? ReadKeyIdentifiers(IDbCommand cmd)
		{
			using(IDataReader rdr = cmd.ExecuteReader())
			{
				if(rdr.Read())
				{
					
					string friendlyName = rdr.IsDBNull(0)? null :rdr.GetString(0);
					string identifier = rdr.IsDBNull(1)? null : rdr.GetString(1);
					
					if(friendlyName == null || identifier == null)
						return null;
						
					return new KeyValuePair<string, string>(friendlyName, identifier);
				}
			}
			
			return null;
		}

		/// <summary>
		/// Creates a parameter for the specified command and properties
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected abstract IDbDataParameter CreateParameter(IDbCommand cmd, string name, DbType type, object value);

		/// <summary>
		/// Derives a parameter name that can be used inside a query (e.g. SqlConnection: paramName -> @paramName, Sqlite paramName -> :paramName)
		/// </summary>
		/// <param name="rawName"></param>
		/// <returns> </returns>
		protected abstract string DeriveParameterName(string rawName);

		public override void Dispose ()
		{
			_connection.Close();
		}

	}
}
