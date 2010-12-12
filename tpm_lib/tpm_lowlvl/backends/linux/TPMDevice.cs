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
using Iaik.Tc.TPM.Lowlevel.Exceptions;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Lowlevel.Backends.Linux
{


	[TPMProvider("linux/device")]
	public class TPMDevice : TPMProvider
	{
		/// <summary>
		/// Using same value than used in trousers
		/// </summary>
		public const int RCV_BUF_SIZE = 2048;
		
		private byte[] _receiveBuffer = new byte[RCV_BUF_SIZE];
		
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

		
		protected override string BackendIdentifier 
		{
			get { return string.Format("{0} -> {1}", ProviderAttributeName, deviceName_); }
		}

		
        /// <summary>
        /// Standard constructor with 4K rx buffer size
        /// </summary>
        public TPMDevice() : this("/dev/tpm0", false)
        {            
        }

        /// <summary>
        /// Special purpose constructor with configurable device name.
        /// </summary>
        /// <param name="deviceName"></param>
        public TPMDevice (String deviceName, bool enableDebugOutput)
			:base(enableDebugOutput)
        {
        	fd_ = -1;
        	deviceName_ = deviceName;
			
			SetupLogger();
        }
		
		public TPMDevice(IDictionary<string, string> parameters)
			:this(parameters["DeviceName"], DictionaryHelper.GetBool("debug", parameters, false))
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
			
			//byte[] rxheader = new byte[10];
	
			int txlen = write(fd_, blob, size);
	        if (txlen < size)
		    	throw new TPMLowLvlException("Failed to write to TPM device " + deviceName_, 2);

			// Read the whole tpm response at once
			int rxlen = read(fd_, _receiveBuffer, _receiveBuffer.Length);

			
			// Read the TPM header
			//int rxlen = read(fd_, rxheader, rxheader.Length);
			if (rxlen < 0)
			   throw new TPMLowLvlException("Failed to read from the TPM device " + deviceName_, 2);
	
			if (rxlen < 10)
	           throw new TPMLowLvlException("Short response (" + rxlen + " bytes) from TPM device " + deviceName_, 3);
	
	
			// Decode the length
			int length = (_receiveBuffer[2] << 24) | (_receiveBuffer[3] << 16) | (_receiveBuffer[4] << 8) | _receiveBuffer[5];
			if (length < 10)
	           throw new TPMLowLvlException("Implausible length response (" + length + " bytes) from TPM device " + deviceName_, 4);
	

			if(length != rxlen)
				throw new TPMLowLvlException("Short payload response (" + rxlen + " bytes ) from TPM device " + deviceName_, 5);                	
	    
			// Already done
			//if (length == rxheader.Length)
			//  return rxheader;
	
            // Need a larger buffer ...
            //byte[] payload = new byte[length - rxheader.Length];
            //rxlen = read(fd_, payload, payload.Length);
			//if (rxlen < (length - rxheader.Length))
	        //    throw new TPMLowLvlException("Short payload response (" + rxlen + " bytes ) from TPM device " + deviceName_, 5);                

			// Assemble the full response buffer
			byte[] rsp = new byte[length];
	
			Array.Copy(_receiveBuffer, 0, rsp, 0, rxlen);
			//Array.Copy(payload, 0, rsp, rxheader.Length, payload.Length);

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
