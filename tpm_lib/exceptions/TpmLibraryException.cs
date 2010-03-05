// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.Tpm.library.exceptions
{
public class TpmLibraryException : Exception
    {
        private uint tddlResult_;
        private byte[] tpmReply_;

        /// <summary>
        /// Constructs a TPM exception based on a given TDDL return code.
        /// </summary>
        /// <param name="tddlResult"></param>
        public TpmLibraryException(uint tddlResult)
            : this("TPM TDDL error", tddlResult)
        {            
        }

        /// <summary>
        /// Constructs a TPM exception based on a given TDDL return code.
        /// </summary>
        /// <param name="tddlResult"></param>
        public TpmLibraryException(String message, uint tddlResult)
            : this(message, tddlResult, null, 0)
        {
        }

        /// <summary>
        /// Constructs a TPM exception based on a given TDDL return code
        /// and a given TPM response blob.
        /// </summary>
        /// <param name="tddlResult"></param>
        /// <param name="reply"></param>
        /// <param name="size"></param>
        public TpmLibraryException(String message, uint tddlResult, byte[] reply, int size)
              : base(message)
        {
            // TDDL status code
            tddlResult_ = tddlResult;

            // Include the TPM reply blob
            if (reply != null)
            {
                tpmReply_ = new byte[size];
                System.Array.Copy(reply, tpmReply_, (int)size);
            }
            else
            {
                tpmReply_ = null;
            }
        }

        /// <summary>
        /// Returns the TDDL return code for this exception.
        /// </summary>
        public uint TddlResult
        {
            get
            {
                return tddlResult_;
            }
        }

        /// <summary>
        /// Returns the TPM response blob for this exception.s        
        /// </summary>
        public byte[] TpmReply
        {
            get
            {
                return tpmReply_;
            }
        }
	}
}    