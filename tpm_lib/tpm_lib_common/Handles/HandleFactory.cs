
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
