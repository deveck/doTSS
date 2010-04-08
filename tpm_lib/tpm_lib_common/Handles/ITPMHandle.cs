
using System;

namespace Iaik.Tc.TPM.Library.Common.Handles
{

	/// <summary>
	/// Defines a resource on the TPM
	/// </summary>
	public interface ITPMHandle
	{
		/// <summary>
		/// Gets the TPM handle identifier
		/// </summary>
		uint Handle { get; }
		
		/// <summary>
		/// Gets the TPM resource type of this handle
		/// </summary>
		TPMResourceType ResourceType { get; }
	}
}
