// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Lowlevel;
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Library.Commands;
using System.Collections.Generic;
using System.IO;

namespace Iaik.Tc.TPM.Library
{


	public class TPM : IDisposable
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
		
		/// <summary>
		/// Returns an indication wether the backend object is opened
		/// </summary>
		public bool IsOpen
		{
			get
			{
				lock(this)
				{
					if(_backend == null)
						return false;
					return _backend.IsOpen;
				}
			}
		}
		
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

		private TPMProvider _backend;
		
//		public TPMProvider backend{
//			get{
//				return this._backend;
//			}
//		}
		
		#region Constructors and initialisation
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
		public void Init (string providerName)
		{
			_backend = TPMProviders.Create(providerName,null);
		}
		
		/// <summary>
		/// Init TPM Connection with a suitable backend, that requiers some options
		/// </summary>
		/// <param name="providerName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="options">
		/// A <see cref="IDictionary<String, String>"/>
		/// </param>
		public void Init (string providerName, IDictionary<String, String> options)
		{
			_backend = TPMProviders.Create(providerName,options);
		}
		
		public void Init (string providerName, IDictionary<String, String> options, StreamWriter debug)
		{
			_backend = TPMProviders.Create(providerName,options);
			_backend.StartDebug(debug);
		}
		
		
		#endregion
		
		public void Open ()
		{
			lock(this){
				if(_isDisposed)
					throw new ObjectDisposedException("TPM object is disposed");
				_backend.Open();
			}			
		}
		
		
		#region IDisposable implementation
		public void Dispose ()
		{
			lock(this)
				if(_isDisposed)
					throw new ObjectDisposedException("TPM object is disposed");
			_backend.Dispose();
			_isDisposed = true;
		}
		#endregion
		
		public TPMCommandResponse Process (TPMCommandRequest request)
		{
			try
			{
				_backend.Open ();
				TPMCommand command = TPMCommandFactory.Create (request.CommandIdentifier);
				command.Init (request.Parameters, _backend);
				return command.Process ();
			}
			finally
			{
				_backend.Close ();
			}
		}
		
	}
}
