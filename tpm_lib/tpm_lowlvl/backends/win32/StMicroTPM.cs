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
// Thanks to Johannes Winter <johannes.winter@TUGraz.at>

using System;
using System.Runtime.InteropServices;
using Iaik.Tc.TPM.Lowlevel.Exceptions;

namespace Iaik.Tc.TPM.Lowlevel.Backends.Win32
{
    /// <summary>
    /// Windows XP wrapper around "tpmddl.dll" shipped as part
    /// of the STMicro TPM driver.
    /// 
    /// The function entry points into the STMicro library are compatible
    /// to the TCG specified TDDL-Interface. Note however, that the calling
    /// convetion is "cdecl" instead of the Windows standard "stdcall" convetion.
    /// </summary>
    [TPMProvider("win32/stmicro")]
    public sealed class StMicroTPM : TPMProvider
    {
        /// <summary>
        /// Receive buffer
        /// </summary>
        private byte[] rxBuffer_;

		
        /// <summary>
        /// Standard constructor with 4K rx buffer size
        /// </summary>
        public StMicroTPM() : this(4096)
        {            
        }

        /// <summary>
        /// Special purpose constructor with configurable rx buffer size
        /// </summary>
        /// <param name="rxbuffersize"></param>
        public StMicroTPM(int rxbuffersize)
        {
            rxBuffer_ = new byte[rxbuffersize];
			SetupLogger();
        }

        /// <summary>
        /// Wrapper for the native TDDL_Open call.
        /// </summary>
        protected override void tpmOpen()
        {
            uint result = TDDL_Open();
            if (result != 0)
                throw new TPMLowLvlException(result);
        }

        /// <summary>
        /// Wrapper for the native TDDL_Close call.
        /// </summary>
        protected override void tpmClose()
        {
            uint result = TDDL_Close();
            if (result != 0)
                throw new TPMLowLvlException(result);
        }

        /// <summary>
        /// Wrapper for the native TDDL_TransmitData call.
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        protected override byte[] tpmTransmit(byte[] blob, int size)
        {
            uint rxlen = (uint)rxBuffer_.Length;

            if (size <= 0)
                throw new InvalidOperationException("Cant transmit empty or negative size blob.");
            
            uint result = TDDL_TransmitData(blob, (uint)size, rxBuffer_, ref rxlen);
            if (result != 0)
            {
                if (rxlen > 0)
					//TODO in each backend!!!
					throw new Exception();
                    //throw new TpmException("TDDL I/O error (partial result)", result, rxBuffer_, (int)rxlen);
                else
                    throw new TPMLowLvlException(result);
            }

            byte[] rxblob = new byte[rxlen];
            System.Array.Copy(rxBuffer_, rxblob, rxlen);
            return rxblob;
        }

        /// <summary>
        /// Native TDDL_Open call. (corresponds to Tddli_Open)        
        /// </summary>
        /// <returns></returns>
        [DllImport("tpmddl.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 TDDL_Open();

        /// <summary>
        /// Native TDDL_Close call. (corresponds to Tddli_Close)
        /// </summary>
        /// <returns></returns>
        [DllImport("tpmddl.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 TDDL_Close();

        /// <summary>
        /// Native TDDL_TransmitData call. (corresponds to Tddli_TransmitData)
        /// </summary>
        /// <param name="inbuf"></param>
        /// <param name="inlen"></param>
        /// <param name="outbuf"></param>
        /// <param name="outlen"></param>
        /// <returns></returns>
        [DllImport("tpmddl.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 TDDL_TransmitData(
               [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] inbuf,
               UInt32 inlen,
               [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] outbuf,
               ref UInt32 outlen);
    }
}