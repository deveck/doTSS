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


﻿using System;
using System.Collections.Generic;
using System.Text;
using Iaik.Tc.TPM.Library.Common.Sign;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.TPM.Library.Commands;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Library.Sign
{
    /// <summary>
    /// This structure provides the mechanism for the TPM to quote the current values of a list of
    /// PCRs.
    /// </summary>
    [TypedStreamSerializable("TPMQuoteInfo")]
    public class TPMQuoteInfoCore : TPMQuoteInfo, ITPMBlobWritable
    {
        public static TPMQuoteInfoCore Create(byte[] compositeHash, byte[] nonce)
        {
            TPMQuoteInfoCore quoteInfo = new TPMQuoteInfoCore();
            quoteInfo._version = CapabilityDataCore.TPMVersionCore.CreateVersion11();
            quoteInfo._compositeHash = compositeHash;
            quoteInfo._nonce = nonce;
            return quoteInfo;
        }

        public TPMQuoteInfoCore()
        {
        }

        public TPMQuoteInfoCore(Stream src)
            :base(src)
        {
        }

        #region ITPMBlobWritable Members

        public void WriteToTpmBlob(TPMBlob blob)
        {
            ((ITPMBlobWritable)_version).WriteToTpmBlob(blob);
            blob.Write(_fixed, 0, _fixed.Length);
            blob.Write(_compositeHash, 0, _compositeHash.Length);
            blob.Write(_nonce, 0, _nonce.Length);
        }

        #endregion
    }
}
