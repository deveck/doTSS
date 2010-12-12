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
// Thanks to Johannes

using System;
using System.IO;
using Iaik.Utils.Serialization;

namespace Iaik.Tc.TPM.Lowlevel.Data
{


	public class TPMBlob : GenericBlob
	{

		public TPMBlob ()
		{
		}
		
		public TPMBlob(byte[] blob) : base(blob, false)
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
		
		#region Reading Data
		public void SkipHeader ()
		{
			Position = 
				  2 //Size of TPM_TAG
				+ 4 //Size of paramSize
				+ 4 //Size of returnCode
				;
		}
		#endregion
		
		
	}
}

