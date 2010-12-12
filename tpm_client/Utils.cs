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
using Iaik.Utils.Hash;
using System.Xml;
using Iaik.Tc.TPM.Context;
using System.Collections.Generic;
using Iaik.Connection.ClientConnections;

namespace Iaik.Tc.TPM
{


	public static class Utils
	{

		public static ProtectedPasswordStorage ReadPassword (string hintText, TPMConsole console, bool retypePw)
		{
			console.Out.Write (hintText);
			
			ConsoleKeyInfo consoleKeyInfo;
			ProtectedPasswordStorage[] pws;
			
			if(retypePw)
				pws = new ProtectedPasswordStorage[] { new ProtectedPasswordStorage (), new ProtectedPasswordStorage () };
			else
				pws = new ProtectedPasswordStorage[] { new ProtectedPasswordStorage() };
			
			for (int i = 0; i < pws.Length; i++)
			{
				ProtectedPasswordStorage pw = pws[i];

				if (i == 1)
					console.Out.Write ("Retype password:");
				
				while (true)
				{
					consoleKeyInfo = console.ReadKeyWithoutEcho ();
					if (consoleKeyInfo.Key == ConsoleKey.Enter)
					{
						console.Out.WriteLine ();
						break;
					}
					else if (consoleKeyInfo.Key == ConsoleKey.Escape)
					{
						console.Out.WriteLine ();
						return null;
					}
					else
						pw.AppendPasswordChar (consoleKeyInfo.KeyChar);
				}
			}

            if (retypePw == false || pws[0].EqualPassword(pws[1]))
            {
                pws[0].Hash();
                return pws[0];
            }
            else
            {
                console.Out.WriteLine("Error: Passwords do not match!");
                return null;
            }
		}
		
		
		
	}
}
