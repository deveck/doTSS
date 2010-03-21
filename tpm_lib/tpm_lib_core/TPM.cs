// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.lowlevel;

namespace Iaik.Tc.Tpm.library
{


	public class TPM : IDisposable
	{
		/// <summary>
		/// Disposed status of the Object
		/// </summary>
		private bool _isDisposed = false;
		
		/// <summary>
		/// Opened/Closed status of the backend
		/// </summary>
		private bool _isOpened = false;
		
		
		
		
		private TPMProvider _backend;
		
		public TPMProvider backend{
			get{
				return this._backend;
			}
		}

		/// <summary>
		/// Default constructor of the object
		/// </summary>
		public TPM(){}
		
		/// <summary>
		/// Init TPM Connection with a suitable backend
		/// </summary>
		/// <param name="providerName">
		/// A <see cref="System.String"/>
		/// </param>
		public void init (string providerName)
		{
			_backend = TpmProviders.Create(providerName,null);
		}
		
		public void Open(){
			lock(this){
				if(_isDisposed)
					throw new ObjectDisposedException("TPM object is disposed");
				_backend.Open();
			}			
		}
		#region IDisposable implementation
		public void Dispose ()
		{
			throw new System.NotImplementedException();
		}
		
		#endregion
	}
}
