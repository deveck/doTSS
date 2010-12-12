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
using Iaik.Tc.TPM.Library.Common.Handles.Authorization;
using Iaik.Tc.TPM.Library.Common.KeyData;

namespace Iaik.Tc.TPM.Library.Common.Handles
{


	public static class HandleFactory
	{

		public static ITPMHandle Create (TPMResourceType resourceType, uint handle)
		{
			if (resourceType == TPMResourceType.TPM_RT_AUTH)
				return new AuthHandle (AuthHandle.AuthType.Unknown, handle);
			else if(resourceType == TPMResourceType.TPM_RT_KEY)
				return new KeyHandle("", handle);
			else
				throw new NotImplementedException (string.Format ("Could not find handle implementation for resource type '{0}'", resourceType));
		}
	}
}
