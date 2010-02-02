using System;
using Iaik.Tc.Tpm.Connection.ClientConnections;

namespace Iaik.Tc.Tpm
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			FrontEndConnection tpmConnection = new NamedPipeConnection("TPM_csharp");
			tpmConnection.Connect();
		}
	}
}
