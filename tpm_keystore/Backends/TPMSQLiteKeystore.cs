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
using Mono.Data.Sqlite;

namespace Iaik.Tc.TPM.Keystore.Backends
{

	/// <summary>
	/// Manages Keys in a Sqlite database file
	/// </summary>
	[TPMKeystore("SQLiteKeystore")]
	public class TPMSQLiteKeystore : AdoKeystore
	{
		

		public TPMSQLiteKeystore(IDictionary<string, string> parameters)
			:base(parameters)
		{
		}


		/// <summary>
		/// Builds the connection to the database and verifies the database scheme
		/// </summary>
		/// <param name="parameters"></param>
		protected override void BuildConnection (IDictionary<string, string> parameters)
		{
			SqliteConnection connection = new SqliteConnection(string.Format("Data Source={0},version=3", parameters["file"]));
			connection.Open();
			_connection = connection;
			
			VerifyScheme();
		}

						
		/// <summary>
		/// Verifies the database scheme
		/// </summary>
		private void VerifyScheme()
		{
			int tableRows;
		
			using(SqliteCommand cmd = (SqliteCommand)BuildCommand("SELECT COUNT(*) FROM sqlite_master WHERE name=:tableName"))
			{
				CreateParameter(cmd, "tableName", DbType.String, "tpm_keys");
			
				tableRows = int.Parse(cmd.ExecuteScalar().ToString());
				
			}
			
			if(tableRows == 0)
			{
				using(IDbCommand cmd = BuildCommand(@"
					CREATE TABLE tpm_keys (
					   friendly_name NVARCHAR(100) NOT NULL PRIMARY KEY ASC,
					   identifier TEXT NOT NULL,
					   creation_date DATETIME NOT NULL,
					   parent_key NVARCHAR(100) NULL,
					   key_data BLOB NOT NULL
					   );"))
				{
					cmd.ExecuteNonQuery();
				}
			}
		}
		
		protected override string DeriveParameterName (string rawName)
		{
			return ":" + rawName;
		}
		
		protected override IDbDataParameter CreateParameter (IDbCommand cmd, string name, DbType dbtype, object value)
		{
			SqliteCommand sCmd = (SqliteCommand)cmd;
			SqliteParameter parameter = (SqliteParameter)sCmd.CreateParameter();
			
			parameter.ParameterName = ":" + name;
			parameter.DbType = dbtype;
			parameter.Value = value;
	
			sCmd.Parameters.Add(parameter);
				
			return parameter;
		}



	}
}
