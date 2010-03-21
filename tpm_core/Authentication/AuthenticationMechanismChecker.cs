using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.TPM.Configuration;
using Iaik.Utils.CommonFactories;
using Iaik.Connection.ClientConnections;

namespace Iaik.Tc.TPM.Authentication
{
    /// <summary>
    /// Checks if a specific AuthenticationMechanism is compatible with a FrontEndConnection
    /// </summary>
    public class AuthenticationMechanismChecker
    {
        /// <summary>
        /// AuthenticationMethod Configuration
        /// </summary>
        private IAuthenticationMethod _authConfig;

        /// <summary>
        /// Type of the authentication mechanism. 
        /// Just enough to ensure that the authentication mechanism is compatible with a given 
        /// FrontEndConnection
        /// </summary>
        private Type _authType;

        /// <summary>
        /// Defines the authentication mechanism constraints
        /// </summary>
        private AuthenticationSettingsAttribute _authSettings;

        public AuthenticationMechanismChecker(IAuthenticationMethod authConfig)
        {
            _authConfig = authConfig;

            _authType = GenericClassIdentifierFactory.FindTypeForIdentifier<AuthenticationMechanism>(authConfig.AuthIdentifier);

            if (_authType == null)
                throw new ArgumentException(string.Format("No AuthenticationMechanism with identifier '{0}' was found", authConfig.AuthIdentifier));

            object[] attributes = _authType.GetCustomAttributes(typeof(AuthenticationSettingsAttribute), false);
            if (attributes == null || attributes.Length == 0)
                throw new ArgumentException(string.Format("Authentication '{0}' does not provide authentication settings!", authConfig.AuthIdentifier));

            _authSettings = (AuthenticationSettingsAttribute)attributes[0];
        }


        /// <summary>
        /// Checks if the AuthenticationMechanism is compatible with the specified connection instance
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public bool IsCompatibleWith(FrontEndConnection connection)
        {
            return IsCompatibleWith(connection.GetType());
        }

        /// <summary>
        /// Checks if the AuthenticationMechanism is compatible with the specified connection type
        /// </summary>
        /// <param name="frontEndConnectionType"></param>
        /// <returns></returns>
        public bool IsCompatibleWith(Type frontEndConnectionType)
        {
            if (_authSettings.HasAssociatedTypes == false)
                return true;

            return _authSettings.HasAssociatedType(frontEndConnectionType);
        }


        /// <summary>
        /// Creates an instance of the specified authentication mechanism
        /// </summary>
        /// <returns></returns>
        public AuthenticationMechanism CreateInstance()
        {
            return GenericClassIdentifierFactory.CreateFromClassIdentifierOrType<AuthenticationMechanism>(_authSettings.Identifier);
        }
    }
}
