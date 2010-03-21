// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Configuration.DotNetConfiguration.Elements;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Configuration
{


	public interface IPermissionMember
	{
		/// <summary>
		/// Gets the type of the corresponding permission entries
		/// </summary>
		IdTypeEnum IdType{get;}
		
		/// <summary>
		/// Gets the Id of the entry
		/// </summary>
		string Id{get;}
		
		/// <summary>
		/// Gets the sub permission (e.g. for user objects it returns the groups)
		/// </summary>
		IEnumerable<IPermissionMember> SubPermissionMembers{get;}
	}
}
