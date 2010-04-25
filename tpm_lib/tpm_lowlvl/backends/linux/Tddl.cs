// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
// Thanks to Johannes Winter <johannes.winter@TUGraz.at>

using System;
using System.Runtime.InteropServices;
using Iaik.Tc.TPM.Lowlevel.Exceptions;

namespace Iaik.Tc.TPM.Lowlevel.Backends.Linux
{

	/// <summary>
    /// Linux wrapper around "libtddl.so.1.2" as shipped as part
    /// of the tpm-emulator package.
    /// </summary>
    [TPMProvider("linux/tddl")]
    public sealed class TddlTpm : TPMProvider
    {
        /// <summary>
        /// Receive buffer
        /// </summary>
        private byte[] rxBuffer_;

        /// <summary>
        /// Standard constructor with 4K rx buffer size
        /// </summary>
        public TddlTpm() : this(4096)
        {            
        }

        /// <summary>
        /// Special purpose constructor with configurable rx buffer size
        /// </summary>
        /// <param name="rxbuffersize"></param>
        public TddlTpm(int rxbuffersize)
        {
            rxBuffer_ = new byte[rxbuffersize];
			SetupLogger();
        }

        /// <summary>
        /// Wrapper for the native TDDL_Open call.
        /// </summary>
        protected override void tpmOpen()
        {
            uint result = Tddli_Open();
            if (result != 0)
					throw new Exception();
                //throw new TpmException(result);
        }

        /// <summary>
        /// Wrapper for the native TDDL_Close call.
        /// </summary>
        protected override void tpmClose()
        {
            uint result = Tddli_Close();
            if (result != 0)
					throw new Exception();
                //throw new TpmException(result);
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
            
            uint result = Tddli_TransmitData(blob, (uint)size, rxBuffer_, ref rxlen);
            if (result != 0)
            {
                if (rxlen > 0)
                    throw new TPMProviderException(string.Format("TDDL I/O error (partial result) result: '{0}'", result));
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
        [DllImport("libtddl.so.1.2", CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 Tddli_Open();

        /// <summary>
        /// Native TDDL_Close call. (corresponds to Tddli_Close)
        /// </summary>
        /// <returns></returns>
        [DllImport("libtddl.so.1.2", CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 Tddli_Close();

        /// <summary>
        /// Native TDDL_TransmitData call. (corresponds to Tddli_TransmitData)
        /// </summary>
        /// <param name="inbuf"></param>
        /// <param name="inlen"></param>
        /// <param name="outbuf"></param>
        /// <param name="outlen"></param>
        /// <returns></returns>
        [DllImport("libtddl.so.1.2", CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 Tddli_TransmitData(
               [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] inbuf,
               UInt32 inlen,
               [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] outbuf,
               ref UInt32 outlen);
    }
}

