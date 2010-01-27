//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iaik.Tc.Tpm.Connection.Packets;

namespace Iaik.Tc.Tpm.Connection
{
    /// <summary>
    /// Abstracts the Connection to the TPM - Library
    /// This is the connection from the clients to the the server or
    /// an accepted connection on the server side
    /// </summary>
    public interface IFrontEndConnection : IDisposable
    {
        /// <summary>
        /// Trys to establish a connection to the TPM Server
        /// </summary>
        /// <remarks>
        /// <para>Throws <see cref="Iaik.Tc.Tpm.Connection.ConnectionFailureException"/>if the connection to the remote host could not be established,
        /// because the remote endpoint is not available</para>
        /// <para>Throws <see cref="Iaik.Tc.Tpm.Connection.ConnectionRejectedException"/>if the connection is rejected by the server because of
        /// some policy settings</para>
        /// <para>Throws <see cref="Iaik.Tc.Tpm.Connection.ConnectionException"/> on some other error</para>
        /// </remarks>
        void Connect();

        /// <summary>
        /// Closes the connection to the remote host
        /// </summary>
        void Close();

        /// <summary>
        /// Transmits the DataPacket to the server, receives and returns the Answer packet.
        /// </summary>
        /// <typeparam name="T">Specifies the type of the return packet to read</typeparam>
        /// <param name="packet"></param>
        /// <returns></returns>
        T Transmit<T>(DataPacket packet) where T : DataPacket;

    }
}
