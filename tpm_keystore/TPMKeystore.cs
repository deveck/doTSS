// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Keystore
{


	/// <summary>
	/// The TPM Keystore class represents the main interface an 
	/// </summary>
	public class TPMKeystore : IDisposable
	{
		
		
		#region Backend status
		/// <summary>
		/// Disposed status of the Object
		/// </summary>
		private bool _isDisposed = false;
		
//		/// <summary>
//		/// Opened/Closed status of the backend
//		/// </summary>
//		private bool _isOpened = false;
		
//		/// <summary>
//		/// Returns an indication wether the backend object is opened
//		/// </summary>
//		public bool IsOpen
//		{
//			get
//			{
//				lock(this)
//				{
//					if(_backend == null)
//						return false;
//					return _backend.IsOpen;
//				}
//			}
//		}
		
		/// <summary>
		/// Returns an indication wether the backend is disposed
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				lock(this)
				{
					return _isDisposed;
				}
			}
		}
		#endregion
		
		private TPMKeystoreProvider _backend = null;

		#region Constructors and initialisation
		/// <summary>
		/// Default constructor of the object
		/// </summary>
		public TPMKeystore ()
		{
		}
		
		public void Init(string providerName, IDictionary<String, String> options)
		{
			_backend = TPMKeystoreProviders.Create(providerName, options);			
		}
		#endregion
		
		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			throw new System.NotImplementedException();
		}
		#endregion
		
	}
}
