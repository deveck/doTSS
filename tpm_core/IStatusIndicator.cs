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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iaik.Tc.TPM
{
    /// <summary>
    /// Interface implemented by classes that indicate the status of any kind of operation
    /// </summary>
    public interface IStatusIndicator
    {
        /// <summary>
        /// Indicates if the operation was successfull
        /// </summary>
        bool Succeeded { get; }

        /// <summary>
        /// Errorcode of the failed operation or null
        /// </summary>
        int? ErrorCode { get; }

        /// <summary>
        /// Human readable error text or null
        /// </summary>
        string ErrorText { get; }
    }
}
