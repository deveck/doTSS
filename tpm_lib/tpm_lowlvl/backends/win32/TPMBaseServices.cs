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
    /// Backend for Windows Vista TPM base services 
    /// (UNTESTED CODE!)
    /// </summary>
    [TPMProvider("win32/tbs")]
    public class TPMBaseServices : TPMProvider
    {
        /// <summary>
        /// Receive buffer
        /// </summary>
        private byte[] rxBuffer_;

        /// <summary>
        /// TBS context handle
        /// </summary>
        private IntPtr hTbsContext_ = new IntPtr();

        /// <summary>
        /// Standard constructor with 4K rx buffer size
        /// </summary>
        public TPMBaseServices() : this(4096)
        {            
        }

        /// <summary>
        /// Special purpose constructor with configurable rx buffer size
        /// </summary>
        /// <param name="rxbuffersize"></param>
        public TPMBaseServices(int rxbuffersize)
        {
            rxBuffer_ = new byte[rxbuffersize];
			SetupLogger();
        }     

        /// <summary>
        /// Wrapper for the native Tbsi_Context_Create call.
        /// </summary>
        protected override void tpmOpen()
        {
            TBS_CONTEXT_PARAMS ctx_params;
            ctx_params.version = 1; /* Guess */
           
            uint result = Tbsi_Context_Create(ref ctx_params, out hTbsContext_);
            if (result != 0)
                throw new TPMLowLvlException("Tbsi_Context_Create failed (" + result + ")", result);
        }

        /// <summary>
        /// Wrapper for the native Tbsi_Submit_Command call.
        /// </summary>
        protected override void tpmClose()
        {
            Tbsip_Context_Close(hTbsContext_);
        }

        /// <summary>
        /// Wrapper for the native Tbsi_Submit_Command call.
        /// </summary>
        protected override byte[] tpmTransmit(byte[] blob, int size)
        {
            uint rxlen = (uint)rxBuffer_.Length;

            if (size <= 0)
                throw new InvalidOperationException("Cant transmit empty or negative size blob.");
                     
            uint result = Tbsip_Submit_Command(hTbsContext_, 
                0 /* Locality */, 
                0 /* Priority */, 
                blob, (uint)size,
                rxBuffer_, ref rxlen);

            if (result != 0)
            {
                if (rxlen > 0)
					//TODO
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
        /// TBS_CONTEXT_PARAMS structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct TBS_CONTEXT_PARAMS
        {
            public UInt32 version;
        };

        /// <summary>
        /// Native Tbsi_Context_Create call. (Corresponds to Tddli_Open)
        /// </summary>        
        [DllImport("tbs.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern UInt32 Tbsi_Context_Create(
             ref TBS_CONTEXT_PARAMS pContextParams,
             out IntPtr phContext
        );

        /// <summary>
        /// Native Tbsip_Context_Close call. (corresponds to Tddli_Close)
        /// </summary>        
        [DllImport("tbs.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern UInt32 Tbsip_Context_Close(IntPtr hContext);

        /// <summary>
        /// Native TDDL_TransmitData call. (corresponds to Tddli_TransmitData)
        /// </summary>        
        [DllImport("tbs.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern UInt32 Tbsip_Submit_Command(
               IntPtr hContext,
               UInt32 locality,
               UInt32 priority,
               [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] inbuf,
               UInt32 inlen,
               [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] byte[] outbuf,
               ref UInt32 outlen);
    }    
}
