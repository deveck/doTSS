using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.Tpm.Commands
{
    /// <summary>
    /// TPM console commands that have this attribute attached, get executed right after console startup,
    /// before the prompt is shown for the first time
    /// </summary>
    public class TPMConsoleStartupCommand : Attribute
    {
    }
}
