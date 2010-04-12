
using System;
using Iaik.Utils.Hash;

namespace Iaik.Tc.TPM
{


	public static class Utils
	{

		public static ProtectedPasswordStorage ReadPassword (string hintText, TPMConsole console)
		{
			console.Out.Write (hintText);
			
			ConsoleKeyInfo consoleKeyInfo;
			ProtectedPasswordStorage[] pws = new ProtectedPasswordStorage[] { new ProtectedPasswordStorage (), new ProtectedPasswordStorage () };
			
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
			
			if (pws[0].Equals (pws[1]))
				return pws[0];
			else
			{
				console.Out.WriteLine ("Error: Passwords do not match!");
				return null;
			}
		}
	}
}
