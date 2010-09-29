// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.IO;
using Iaik.Utils.Hash;

namespace Iaik.Utils
{


	public static class ConsoleUtils
	{

		public static ProtectedPasswordStorage ReadPassword (string hintText)
		{
			Console.Write (hintText);
			
			ConsoleKeyInfo consoleKeyInfo;
			ProtectedPasswordStorage pws = new ProtectedPasswordStorage();
			
			
				while (true)
				{
					consoleKeyInfo = Console.ReadKey(true);
					if (consoleKeyInfo.Key == ConsoleKey.Enter)
					{
						Console.WriteLine ();
						return pws;
					}
					else if (consoleKeyInfo.Key == ConsoleKey.Escape)
					{
						Console.WriteLine ();
						return null;
					}
					else
						pws.AppendPasswordChar (consoleKeyInfo.KeyChar);
				}
			}

		}
		
		
		
	}

