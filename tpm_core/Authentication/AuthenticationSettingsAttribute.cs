// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.ClientConnections;
using Iaik.Utils.CommonAttributes;

namespace Iaik.Tc.TPM.Authentication
{

    /// <summary>
    /// Attribute attached to <see>AuthenticationMechanism</see>
    /// </summary>
    /// <remarks>
    /// Every authentication method needs to have this attribute attached. Specify the authentication types that are compatible with this authemtication mechanism
    /// </remarks>
	public class AuthenticationSettingsAttribute : ClassIdentifierAttribute
	{		
		private Type[] _associatedConnections = null;
		
		/// <summary>
		/// Types derived from <see>FrontEndConnection</see> that are compatible
		/// with the marked Authentication mechanism.
		/// If no associated connections are supplied, the marked authentication mechanism can
		/// be used for all connections
		/// </summary>
		public Type[] AssociatedConnections
		{
			get{ return _associatedConnections;}
		}
		
	
		/// <summary>
		/// Checks if this attribute has associated <see>FrontEndConnections</see>
		/// </summary>
		public bool HasAssociatedTypes
		{
			get{ return _associatedConnections != null && _associatedConnections.Length > 0;}
		}
		
		public AuthenticationSettingsAttribute (string name, params Type[] associatedConnections)
            :base(name)
		{
			if(associatedConnections != null && associatedConnections.Length > 0)
			{
				foreach(Type t in associatedConnections)
				{
					if(typeof(FrontEndConnection).IsAssignableFrom(t) == false)
						throw new ArgumentException(string.Format("{0} is not a FrontEndConnection", t));
				}
				
				
				_associatedConnections = associatedConnections;
			}			
		}

		
		/// <summary>
		/// Checks if the <see>FrontEndConnection</see> is associated with this attribute
		/// </summary>
		/// <returns />
		public bool HasAssociatedType<T>() where T:FrontEndConnection
		{
			return HasAssociatedType(typeof(T));
		}
		
		/// <summary>
		/// Checks if this attribute is associated with the given Type(<see>FrontEndConnection</see>)
		/// </summary>
		/// <param name="t" />
		/// <returns />
		public bool HasAssociatedType(Type t)
		{
			if(_associatedConnections != null)
			{
				foreach(Type associatedConnection in _associatedConnections)
				{
					if(associatedConnection.Equals(t))
						return true;
				}
			}
			
			return false;
		}
		
		
	}
}
