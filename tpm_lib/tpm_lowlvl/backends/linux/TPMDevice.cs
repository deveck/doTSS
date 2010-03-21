// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
// Thanks to Johannes Winter <johannes.winter@TUGraz.at>

using System;
using Iaik.Tc.TPM.Lowlevel.Exceptions;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Lowlevel.Backends.Linux
{


	[TPMProvider("linux/device")]
	public class TPMDevice : TPMProvider
	{

		/// <summary>
		/// Read/write flag for open.
		/// </summary>
		private const int O_RDWR = 0x0002;
	
        /// <summary>
        /// Device file name
        /// </summary>
		private String deviceName_;
	
		/// <summary>
		/// File handle.
		/// </summary>
		private int fd_;

        /// <summary>
        /// Standard constructor with 4K rx buffer size
        /// </summary>
        public TPMDevice() : this("/dev/tpm0")
        {            
        }

        /// <summary>
        /// Special purpose constructor with configurable device name.
        /// </summary>
        /// <param name="deviceName"></param>
        public TPMDevice (String deviceName)
        {
        	fd_ = -1;
        	deviceName_ = deviceName;
        }
		
		public TPMDevice(IDictionary<string, string> parameters)
			:this(parameters["DeviceName"])
		{
		}

        /// <summary>
        /// Wrapper for the native TDDL_Open call.
        /// </summary>
        protected override void tpmOpen()
        {
		fd_ = open(deviceName_, O_RDWR); 
		if (fd_ < 0)
		   throw new TPMLowLvlException("Failed to open TPM device " + deviceName_, 1);
        }

        /// <summary>
        /// Wrapper for the native TDDL_Close call.
        /// </summary>
        protected override void tpmClose()
        {
		close(fd_);
        }

        /// <summary>
        /// Wrapper for the native TDDL_TransmitData call.
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        protected override byte[] tpmTransmit(byte[] blob, int size)
        {
			byte[] rxheader = new byte[10];
	
			int txlen = write(fd_, blob, size);
	        if (txlen < size)
		    	throw new TPMLowLvlException("Failed to write to TPM device " + deviceName_, 2);
	
			// Read the TPM header
			int rxlen = read(fd_, rxheader, rxheader.Length);
			if (rxlen < 0)
			   throw new TPMLowLvlException("Failed to read from the TPM device " + deviceName_, 2);
	
			if (rxlen < 10)
	           throw new TPMLowLvlException("Short response (" + rxlen + " bytes) from TPM device " + deviceName_, 3);
	
	
			// Decode the length
			int length = (rxheader[2] << 24) | (rxheader[3] << 16) | (rxheader[4] << 8) | rxheader[5];
			if (length < 10)
	           throw new TPMLowLvlException("Implausible length response (" + length + " bytes) from TPM device " + deviceName_, 4);
	
	        // Already done
			if (length == rxheader.Length)
			  return rxheader;
	
            // Need a larger buffer ...
            byte[] payload = new byte[length - rxheader.Length];
            rxlen = read(fd_, payload, payload.Length);
			if (rxlen < (length - rxheader.Length))
	            throw new TPMLowLvlException("Short payload response (" + rxlen + " bytes ) from TPM device " + deviceName_, 5);                

			// Assemble the full response buffer
			byte[] rsp = new byte[length];
	
			Array.Copy(rxheader, 0, rsp, 0, rxheader.Length);
			Array.Copy(payload, 0, rsp, rxheader.Length, payload.Length);

            return rsp;
        }

        /// <summary>
        /// Native TDDL_Open call. (corresponds to Tddli_Open)        
        /// </summary>
        /// <returns></returns>
        [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
        private static extern int open([MarshalAs(UnmanagedType.LPStr)] String name, int flags);

        /// <summary>
        /// Native TDDL_Close call. (corresponds to Tddli_Close)
        /// </summary>
        /// <returns></returns>
        [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
        private static extern int close(int fd);

        /// <summary>
        /// Native TDDL_TransmitData call. (corresponds to Tddli_TransmitData)
        /// </summary>
        /// <param name="inbuf"></param>
        /// <param name="inlen"></param>
        /// <param name="outbuf"></param>
        /// <param name="outlen"></param>
        /// <returns></returns>
        [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
        private static extern int write(int hfile,
	       [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] inbuf,
               int count);


        /// <summary>
        /// Native TDDL_TransmitData call. (corresponds to Tddli_TransmitData)
        /// </summary>
        /// <param name="inbuf"></param>
        /// <param name="inlen"></param>
        /// <param name="outbuf"></param>
        /// <param name="outlen"></param>
        /// <returns></returns>
        [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
        private static extern int read(int hfile,
	       [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] outbuf,
               int count);
	}
}
