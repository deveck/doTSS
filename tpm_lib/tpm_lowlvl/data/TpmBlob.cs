// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
// Thanks to Johannes

using System;
using System.IO;
using Iaik.Tc.Tpm.lowlevel.data;

namespace Iaik.Tc.Tpm.lowlevel.data
{


	public class TpmBlob : GenericBlob
	{

		public TpmBlob ()
		{
		}

		#region Writing data
        /// <summary>
        /// Write a TPM command header (with size 0, assuming that
        /// WriteCmdSize will be called later)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ordinal"></param>
        public void WriteCmdHeader(UInt16 tag, UInt32 ordinal)
        {
            WriteCmdHeader(tag, ordinal, 0);
        }

		
		// TODO: LEAVE IT???
        /// <summary>
        /// Write an object to the TPM blob
        /// </summary>
        /// <param name="obj"></param>
        public void Write(ITpmBlobSerializable obj)
        {
            obj.WriteToBlob(this);
        }

        /// <summary>
        /// Write the TPM command header
        /// </summary>
        /// <param name="?"></param>
        public void WriteCmdHeader(UInt16 tag, UInt32 ordinal, int size)
        {
            WriteUInt16(tag);
            WriteUInt32((UInt32)size);
            WriteUInt32(ordinal);            
        }

        /// <summary>
        /// Backpatch the command size
        /// </summary>
        public void WriteCmdSize()
        {            
            long current = this.Position;

            Position = 2;
            WriteUInt32((UInt32)this.Length);
            Position = current;
        }

        #endregion
		
		
	}
}

