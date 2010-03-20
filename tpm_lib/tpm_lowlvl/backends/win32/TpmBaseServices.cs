// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
// Thanks to Johannes Winter <johannes.winter@TUGraz.at>

using System;
using System.Runtime.InteropServices;
using Iaik.Tc.Tpm.lowlevel.exceptions;



namespace Iaik.Tc.Tpm.lowlevel.backends.win32
{
    /// <summary>
    /// Backend for Windows Vista TPM base services 
    /// (UNTESTED CODE!)
    /// </summary>
    [TpmProvider("win32/tbs")]
    public class TpmBaseServices : TPMProvider
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
        public TpmBaseServices() : this(4096)
        {            
        }

        /// <summary>
        /// Special purpose constructor with configurable rx buffer size
        /// </summary>
        /// <param name="rxbuffersize"></param>
        public TpmBaseServices(int rxbuffersize)
        {
            rxBuffer_ = new byte[rxbuffersize];
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
                throw new TpmLowLvlException("Tbsi_Context_Create failed (" + result + ")", result);
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
                    throw new TpmLowLvlException(result);
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
