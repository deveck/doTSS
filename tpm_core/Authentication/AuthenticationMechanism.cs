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


// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Context;
using Iaik.Tc.TPM.Subsystems.Authentication;
using Iaik.Tc.TPM.Subsystems;

namespace Iaik.Tc.TPM.Authentication
{

	/// <summary>
	/// Base class for all Authentication mechanisms (implicit and explicit)
	/// </summary>
	/// <remarks>
	/// Derived classes need to have the <see>AuthenticationSettingsAttribute</see> defined
	/// so the framework can figure out implicit authentication mechanisms.
	/// Otherwise a <see>NotSupportedException</see> is thrown
	///</remarks>
	public abstract class AuthenticationMechanism
	{
		/// <summary>
		/// The context in which this mechanism is used
		/// </summary>
		protected EndpointContext _context;
		
		/// <summary>
		/// Gets the unique name of this mechanism.
		/// Defined in the AuthenticationSettingsAttribute
		/// </summary>
		public string  Name
		{ 
			get
			{ 
				//Bounds and type checking is not necessary here because it is done in the ctor
                return ((AuthenticationSettingsAttribute)this.GetType().
                        GetCustomAttributes(typeof(AuthenticationSettingsAttribute), false)[0]).Identifier;
			}			
		}

	
		public AuthenticationMechanism()
		{
			//Checks if the resulting type has the AuthenticationSettingsAttribute defined
			object[] attributes = this.GetType().GetCustomAttributes(typeof(AuthenticationSettingsAttribute), false);
			if(attributes == null || attributes.Length == 0)
				throw new NotSupportedException("AuthenticationSettingsAttribute is not defined");
			else if(attributes.Length > 1)
				throw new NotSupportedException("AuthenticationSettingsAttribute is defined more than once");
		}
		
		/// <summary>
        /// Initializes/prepares the context for use
        /// </summary>
        /// <param name="context"></param>
        public virtual void Initialize(EndpointContext context)
        {
            _context = context;
        }
		
		/// <summary>
		///Starts the authentication process 
		/// </summary>
		/// <remarks>
		/// This method should not block until the process has finished
		/// </remarks>
		public abstract void Authenticate(RequestContext<AuthenticateRequest, AuthenticateResponse> request);
	}
}
