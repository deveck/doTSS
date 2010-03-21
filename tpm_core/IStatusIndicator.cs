using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.TPM
{
    /// <summary>
    /// Interface implemented by classes that indicate the status of any kind of operation
    /// </summary>
    public interface IStatusIndicator
    {
        /// <summary>
        /// Indicates if the operation was successfull
        /// </summary>
        bool Succeeded { get; }

        /// <summary>
        /// Errorcode of the failed operation or null
        /// </summary>
        int? ErrorCode { get; }

        /// <summary>
        /// Human readable error text or null
        /// </summary>
        string ErrorText { get; }
    }
}
