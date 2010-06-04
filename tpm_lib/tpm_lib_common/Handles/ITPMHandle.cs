
using System;

namespace Iaik.Tc.TPM.Library.Common.Handles
{

	/// <summary>
	/// Defines a resource on the TPM
	/// </summary>
	public interface ITPMHandle : ITypedParameter
	{
		/// <summary>
		/// Gets the tpm context blob
		/// </summary>
		byte[] ContextBlob{get;}
		
		/// <summary>
		/// Forces LoadContext to use Handle as new handle (OSAP)
		/// </summary>
		bool ForceHandle{ get; }
		
		/// <summary>
		/// Gets the TPM handle identifier
		/// </summary>
		uint Handle { get; set;}
		
		/// <summary>
		/// Gets the TPM resource type of this handle
		/// </summary>
		TPMResourceType ResourceType { get; }
	}
}
