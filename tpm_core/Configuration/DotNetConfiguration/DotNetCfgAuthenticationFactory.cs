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
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;
using System.Reflection;

namespace Iaik.Tc.TPM.Configuration.DotNetConfiguration
{

	public static class DotNetCfgAuthenticationFactory
	{

		public static Authentication CreateAuthentication(AuthenticationType authenticationType)
		{
			foreach(Type t in Assembly.GetExecutingAssembly().GetTypes())
			{
				if(   t.IsAbstract == false 
				   && t.IsClass == true
				   && typeof(Authentication).IsAssignableFrom(t)
				   )
				{
					//we got a serious candidate, now just check if the 
					//DotNetCfgAuthenticationAttribute is defined
					
					object[] attrs = t.GetCustomAttributes(typeof(DotNetCfgAuthenticationAttribute), false);
					ConstructorInfo ctor = null;
					if(	  attrs != null 
					   && attrs.Length == 1 
					   && (attrs[0] as DotNetCfgAuthenticationAttribute).AuthenticationName.Equals(authenticationType.Type)
					   && (ctor = t.GetConstructor(new Type[]{typeof(AuthenticationType)})) != null)
					                                                                               
					{
						return (Authentication)ctor.Invoke(new object[]{authenticationType});
					}
				}
			}
			
			return null;
		}
		
		
	}
}
