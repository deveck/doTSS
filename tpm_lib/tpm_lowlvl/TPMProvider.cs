// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.IO;
using Iaik.Tc.Tpm.lowlevel.data;

namespace Iaik.Tc.Tpm.lowlevel
{
	
	 /// <summary>
    /// Low-level interface to a TPM.
    /// 
    /// This class provides a synchronized TDDL-style 
    /// C# interface to the TPM.    
    /// </summary>
    public abstract class TPMProvider : IDisposable
    {
        /// <summary>
        /// Open/close status for the TPM.
        /// </summary>
        private bool isOpen_ = false;

        /// <summary>
        /// Disposal status for this object
        /// </summary>
        private bool isDisposed_ = false;

        /// <summary>
        /// Constructs a new closed TPM object.
        /// </summary>
        public TPMProvider()
        {            
        }

        /// <summary>
        /// Opens the low-level connection to the TPM.
        /// </summary>
        public void Open()
        {
            lock (this)
            {
                if (isDisposed_)
                    throw new ObjectDisposedException("TPM");                
                
                if (!isOpen_)
                {
                    tpmOpen();                    
                    isOpen_ = true;
                }
            }
        }

        /// <summary>
        /// Closes the low-level connection to the TPM.
        /// </summary>
        public void Close()
        {
            lock (this)
            {
                if (isDisposed_)
                    throw new ObjectDisposedException("TPM");

                if (isOpen_) {
                    tpmClose();
                    isOpen_ = false;
                }                
            }
        }

        #region TPM communication interface
        /// <summary>
        /// Transmits a byte blob to the TPM and returns the
        /// response blob.                
        /// </summary>
        /// <param name="blob">Blob to transmit</param>       
        /// <returns></returns>
        public byte[] Transmit(byte[] blob)
        {
            return Transmit(blob, blob.Length);
        }

        /// <summary>
        /// Transmits a byte blob to the TPM and returns the
        /// response blob.                
        /// </summary>
        /// <param name="blob">Blob to transmit</param>
        /// <param name="size">Number of bytes to transmit</param>
        /// <returns></returns>
        public byte[] Transmit(byte[] blob, int size)
        {
            lock (this)
            {
                if (isDisposed_)
                    throw new ObjectDisposedException("TPM");

                if (!isOpen_)
                    throw new InvalidOperationException("TPM is closed");

                return tpmTransmit(blob, size);
            }
        }

        /// <summary>
        /// Transmit support for TpmMemoryStreams.
        /// </summary>
        /// <param name="inblob"></param>
        /// <returns></returns>
        public TpmBlob Transmit(TpmBlob instm)
        {
            return Transmit(instm, true);
        }

        /// <summary>
        /// Transmit support for TpmMemoryStreams.
        /// </summary>
        /// <param name="instm"></param>
        /// <param name="writeSize"></param>
        /// <returns></returns>
        public TpmBlob Transmit(TpmBlob instm, bool writeSize)
        {
            if (writeSize)                
                instm.WriteCmdSize();

            byte[] inblob = instm.GetBuffer();
            byte[] outblob = Transmit(inblob, (int)instm.Length);
            return new TpmBlob(outblob);
        }

        /// <summary>
        /// Transmit the and check the given TPM blob.
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="writeSize"></param>
        /// <returns></returns>
        public TpmBlob TransmitAndCheck(TpmBlob blob)
        {
            return TransmitAndCheck(blob, true);
        }

        /// <summary>
        /// Transmit and check the given TPM blob.
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        public TpmBlob TransmitAndCheck(TpmBlob blob, bool writeSize)
        {            
            ushort expected_rsp_tag;

            // Determine the expected RSP tag
            blob.Position = 0;
            switch (blob.ReadUInt16())
            {
                case TpmCmdTags.TPM_TAG_RQU_COMMAND:
                    expected_rsp_tag = TpmCmdTags.TPM_TAG_RSP_COMMAND;
                    break;

                case TpmCmdTags.TPM_TAG_RQU_AUTH1_COMMAND:
                    expected_rsp_tag = TpmCmdTags.TPM_TAG_RQU_AUTH1_COMMAND;
                    break;

                case TpmCmdTags.TPM_TAG_RQU_AUTH2_COMMAND:
                    expected_rsp_tag = TpmCmdTags.TPM_TAG_RQU_AUTH2_COMMAND;
                    break;

                default:
				throw new Exception();
                    //throw new TpmCommandException("Unsupported TPM request tag", blob);
            }

            // Do the actual transaction
            TpmBlob rsp = Transmit(blob, writeSize);

            // Check the response
            CheckTpmReponse(rsp, expected_rsp_tag);

            return rsp;
        }

        /// <summary>
        /// Returns an indication wether this TPM object is open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                lock (this)
                {
                    return isOpen_;
                }
            }
        }
        #endregion

        #region Low-Level communication
        /// <summary>
        /// Implementation specific function for opening the TPM connection.
        /// 
        /// This method is called with the monitor for the TPM object (this) locked.
        /// (i.a. the derived class does not have to care for synchronizatron with
        /// respect to "this")
        /// </summary>
        protected abstract void tpmOpen();

        /// <summary>
        /// Implementation specific function for closing the TPM connection.
        /// 
        /// This method is called with the monitor for the TPM object (this) locked.
        /// (i.a. the derived class does not have to care for synchronizatron with
        /// respect to "this")
        /// </summary>
        protected abstract void tpmClose();

        /// <summary>
        /// Implementation specific function for transmitting data over
        /// the TPM connection.
        /// 
        /// This method is called with the monitor for the TPM object (this) locked.
        /// (i.a. the derived class does not have to care for synchronizatron with
        /// respect to "this")
        /// </summary>
        /// <param name="blob">The request blob being sent to the TPM</param>
        /// <returns>The response blob generated by the TPM</returns>
        protected abstract byte[] tpmTransmit(byte[] blob, int size);
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose this TPM object.
        /// 
        /// This method takes care of disposing the TPM object and closes
        /// the underlying low-level connection.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (isDisposed_)
                    throw new ObjectDisposedException("TPM");

                try
                {
                    if (isOpen_)
                        tpmClose();
                }               
                finally
                {
                    isOpen_ = false;
                    isDisposed_ = true;
                }
            }            
        }
        #endregion

        #region TPM Command handling helpers
        /// <summary>
        /// Check a TPM reply blob
        /// </summary>
        /// <param name="reply">The tag type of this blob</param>
        public UInt16 CheckTpmReponse(TpmBlob reply)
        {            
            if (reply.Length < 10)
				throw new Exception();
                //throw new TpmCommandException("Short TPM response", reply);

            // Start from position zero
            reply.Position = 0;

            // Check the reply tag
            ushort replyTag = reply.ReadUInt16();
            if (replyTag != TpmCmdTags.TPM_TAG_RSP_COMMAND &&
                replyTag != TpmCmdTags.TPM_TAG_RSP_AUTH1_COMMAND &&
                replyTag != TpmCmdTags.TPM_TAG_RSP_AUTH2_COMMAND)
            {
                throw new Exception();
				//throw new TpmCommandException("Invalid TPM response tag", reply);
            }

            // Check the parameter size
            uint paramSize = reply.ReadUInt32();
            if ((int)paramSize != reply.Length)
            {
				throw new Exception();
                //throw new TpmCommandException("Bad TPM response paramSize", reply);
            }

            // Finally check the TPM result
            uint tpmResult = reply.ReadUInt32();
            if (tpmResult != 0)
            {
				throw new Exception();
                //throw new TpmCommandFailedException(tpmResult, reply);
            }

            return replyTag;
        }

        /// <summary>
        /// TPM response check with tag test
        /// </summary>
        /// <param name="rsp"></param>
        /// <param name="p"></param>
        public void CheckTpmReponse(TpmBlob rsp, ushort expected_tag)
        {
            if (CheckTpmReponse(rsp) != expected_tag)
               throw new Exception();
				// throw new TpmCommandException("TPM response tag does not match request tag", rsp);
        }
        #endregion

        #region 16. Integrity Collection and Reporting
        /// <summary>
        /// TPM_PcrRead - Read a the current value of a PCR.
        /// </summary>
        /// <param name="pcrIndex"></param>
        /// <returns></returns>
        public byte[] PcrRead(UInt32 pcrIndex)
        {
			// TODO!!!!
            //TpmBlob req = TpmBlob.CreateRquCommand(TpmOrdinals.TPM_ORD_PcrRead);
            //req.WriteUInt32(pcrIndex);

            //TpmBlob rsp = TransmitAndCheck(req);

            // Return the PCR value
        //    return rsp.ReadBytes(20);
			return null;
        }

        /// <summary>
        /// TPM_Extend - Extend a PCR.
        /// </summary>
        /// <param name="pcrIndex"></param>
        /// <param name="inDigest"></param>
        /// <returns></returns>
        public byte[] PcrExtend(UInt32 pcrIndex, byte[] inDigest)
        {
            if (inDigest.Length != 20)
                throw new ArgumentException("inDigest must be exactly 20 bytes");

            //TpmBlob req = TpmBlob.CreateRquCommand(TpmOrdinals.TPM_ORD_Extend);
//            req.WriteUInt32(pcrIndex);
//            req.Write(inDigest, 0, 20);
//
//            TpmBlob rsp = TransmitAndCheck(req);
//
//            // Return the generated PCR value
//            return rsp.ReadBytes(20);
			return null;
        }

//        /// <summary>
//        /// TPM_PCR_Reset - Resets the selected (resettable) PCRs.
//        /// </summary>
//        /// <param name="pcrs"></param>
//        public void PcrReset(TpmPcrSelection pcrs)
//        {
//            TpmBlob req = TpmBlob.CreateRquCommand(TpmOrdinals.TPM_ORD_PCR_Reset);
//            req.Write(pcrs);
//
//            TransmitAndCheck(req);            
//        }
        #endregion

	#region Helper functions for blob reception and transfer over streams
	/// <summary>
        /// Receive a raw TPM blob
        /// </summary>
        /// <param name="stm"></param>
        /// <returns></returns>
        public static byte[] RawReceiveBlob(Stream stm)
        {
            byte[] header = new byte[6];
            int in_bytes = stm.Read(header, 0, 6);
            if (in_bytes < 6)
                throw new EndOfStreamException("End of stream while receiving TPM blob header " + in_bytes);

            // Decode the header
            uint size = (uint)((header[2] << 24) |
                (header[3] << 16) | (header[4] << 8) | (header[5]));


            byte[] full_blob;

            if (size > 6)
            {
                // Receive and decode the body
                full_blob = new byte[(int)size];
                Array.Copy(header, 0, full_blob, 0, 6);

                in_bytes = stm.Read(full_blob, 6, (int)(size - 6));
                if (in_bytes < (size - 6))
                {
                    throw new EndOfStreamException("End of stream while receiving TPM blob body");
                }
            }
            else
            {
                // Just a simple command without body
                full_blob = header;
            }

            return full_blob;
        }

        /// <summary>
        /// Receive a TPM blob from the given network stream.        
        /// </summary>
        /// <param name="nstrm"></param>
        /// <returns></returns>
        public static TpmBlob ReceiveBlob(Stream stm)
        {           
            return new TpmBlob(RawReceiveBlob(stm));
        }
       
         /// <summary>
        /// Raw blob transmission
        /// </summary>
        /// <param name="stm"></param>
        /// <param name="data"></param>
        /// <param name="size"></param>
        public static void RawSendBlob(Stream stm, byte[] data, int size)
        {
            // And go!
            stm.Write(data, 0, size);
	    stm.Flush();
        }

        /// <summary>
        /// Transmit a blob to the given network stream
        /// </summary>
        /// <param name="stm"></param>
        /// <param name="tpmBlob"></param>
        public static void SendBlob(Stream stm, TpmBlob tpmBlob)
        {
            // We need to have a valid size when working over the network
            // Disabled for now - tpmBlob.WriteCmdSize();
            //tpmBlob.WriteCmdSize();
            RawSendBlob(stm, tpmBlob.GetBuffer(), (int)tpmBlob.Length);
        }

	#endregion
    }
}
