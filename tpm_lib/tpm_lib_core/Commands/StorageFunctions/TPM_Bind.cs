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

using System;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Common.Storage;
using Iaik.Tc.TPM.Library.Storage;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Library.Commands.StorageFunctions
{

	[TPMCommands(TPMCommandNames.TPM_CMD_Bind)]
	public class TPM_Bind : TPMCommand
	{

		public override TPMCommandResponse Process ()
		{
			if(_params.GetValueOf<string>("type", "") == "request_prefix")
			{
				
				TPMBoundDataCore boundData = TPMBoundDataCore.Encapsulate(new byte[0]);
				
				_responseParameters = new Parameters();
				using(TPMBlob blob = new TPMBlob())
				{
					boundData.WriteToTpmBlob(blob);
					_responseParameters.AddPrimitiveType("prefix", blob.ToArray());
				}
				
				return new TPMCommandResponse(true, TPMCommandNames.TPM_CMD_Bind, _responseParameters);
			}
			else 
				throw new ArgumentException("TPM_Bind: did not find valid type");
		}

	}
}
