// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.Tpm.Configuration.DotNetConfiguration.Elements;
using System.Reflection;

namespace Iaik.Tc.Tpm.Configuration.DotNetConfiguration
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
