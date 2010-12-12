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
using System.Linq;
using System.Text;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.Common.Sign
{
    /// <summary>
    /// This structure provides the mechanism for the TPM to quote the current values of a list of
    /// PCRs.
    /// </summary>
    [TypedStreamSerializable("TPMQuoteInfo")]
    public class TPMQuoteInfo : AutoStreamSerializable, ITypedParameter
    {

        [SerializeMe(0)]
        protected CapabilityData.TPMVersion _version;

        /// <summary>
        /// This MUST be 1.1.0.0
        /// </summary>
        public CapabilityData.TPMVersion Version
        {
            get { return _version; }
            set { _version = value; }
        }

        
        [SerializeMe(1)]
        protected byte[] _fixed = Encoding.ASCII.GetBytes("QUOT");

        /// <summary>
        /// This SHALL always be the string ‘QUOT’
        /// </summary>
        public byte[] Fixed
        {
            get { return _fixed; }
            set { _fixed = value; }
        }

        
        [SerializeMe(2)]
        protected byte[] _compositeHash;

        /// <summary>
        /// This SHALL be the result of the composite hash algorithm using the current values of the
        /// requested PCR indices.
        /// </summary>
        public byte[] CompositeHash
        {
            get { return _compositeHash; }
            set { _compositeHash = value; }
        }

        
        [SerializeMe(3)]
        protected byte[] _nonce;

        /// <summary>
        /// 160 bits of externally supplied data
        /// </summary>
        public byte[] Nonce
        {
            get { return _nonce; }
            set { _nonce = value; }
        }

        public TPMQuoteInfo()
        {
        }

        public TPMQuoteInfo(Stream src)
        {
            Read(src);
        }
    }
}
