//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Context;
using Iaik.Tc.Tpm.Connection.ClientConnections;

namespace Iaik.Tc.Tpm.Connection
{
    /// <summary>
    /// Delegate used to indicate that a client connected to the server
    /// </summary>
    /// <param name="client"></param>
    public delegate void ClientConnectedDelegate(FrontEndConnection connection);

    /// <summary>
    /// Abstracts the listening connection on the server side.
    /// there is typically only one listeniong connection of each 
    /// supported type on the server
    /// </summary>
    public interface IFrontEndServerListener : IDisposable
    {
        /// <summary>
        /// Indicates that a client connected to the server
        /// Attention: This event can be raised in context of ANY thread,
        /// thread synchroization needs to be done outside
        /// </summary>
        event ClientConnectedDelegate ClientConnected;

        /// <summary>
        /// Starts/Resumes the listener
        /// </summary>
        void Listen();

        /// <summary>
        /// Suspends the listener, no further connections are accepted.
        /// Already existing connections are not affected in any way
        /// </summary>
        void SuspendListener();

    }
}
