//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.Threading;

namespace Iaik.Tc.Tpm.Connection.ServerListeners
{
    /// <summary>
    /// Implements the IFrontEndServerListener for NamedPipes compatible
    /// with Win32
    /// </summary>
    /// <remarks>
    /// To be able to support multiple client connections at once, it is possible
    /// to start multiple NamedPipeServerStreams with the same pipe name, these opened pipes
    /// get populated one after another, just as the client "connections" come in.
    /// This could be seen as a simulation of a TCP connection listener! :-)
    /// </remarks>
    public class NamedPipeListener : IFrontEndServerListener
    {
        

        /// <summary>
        /// Specifies the maximum number of clients that can be connected at once to the pipe server.
        /// This value is also dependent on the operating system because not all systems might
        /// support the same number of named pipe connections
        /// </summary>
        private int _maxClients = 10;

        /// <summary>
        /// Specifies the minimum number of idle listening threads. 
        /// If this value is null all listening threads are started on startup, if not only _alwaysListeningClients-Listener
        /// threads are started, once a client connects another listening thread is started.
        /// </summary>
        private int? _alwaysListeningClients = 4;

        /// <summary>
        /// Specifies the unique pipe name
        /// </summary>
        private string _pipeName = "TPM_csharp";

        /// <summary>
        /// Indicates if this Listener has been disposed and is not usabel any further
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Contains all currently active listeners
        /// </summary>
        private List<ListenerInfo> _listeners = new List<ListenerInfo>();

        #region IFrontEndServerListener Members

        public event ClientConnectedDelegate ClientConnected;
        
        public void Listen()
        {
            lock (_listeners)
            {
                int listenersToStart = Math.Max(0, _maxClients - _listeners.Count);

                if (_alwaysListeningClients != null)
                    listenersToStart = Math.Min(_alwaysListeningClients.Value, listenersToStart);


                for (int listenerCounter = 0; listenerCounter < listenersToStart; listenerCounter++)
                {
                    Thread listenerThread = new Thread(new ThreadStart(ListenerThreadProc));
                    _listeners.Add(new ListenerInfo(DateTime.Now, listenerThread));
                    listenerThread.Start();
                }
            }
        }

        public void SuspendListener()
        {
            lock (_listeners)
            {
                foreach (ListenerInfo listenerInfo in _listeners)
                    listenerInfo.ListenerThread.Abort();

                _listeners.Clear();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_disposed == false)
            {
                SuspendListener();
                _disposed = true;
            }
        }

        #endregion

        /// <summary>
        /// Thread method for a single listener thread
        /// </summary>
        private void ListenerThreadProc()
        {
            try
            {
                NamedPipeServerStream serverPipe = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, _maxClients);
                serverPipe.WaitForConnection();

                //Once the client is connected we "convert" the listener to a simple connection,
                //there is no need to know afterwards that this was a Listener...long ago...

                
            }
            catch(ThreadAbortException ex)
            {
            }
        }


        
        /// <summary>
        /// Holds some information about a single listener thread
        /// </summary>
        private struct ListenerInfo
        {
            /// <summary>
            /// Specifies the start time of the listener thread
            /// </summary>
            public DateTime StartTime;

            /// <summary>
            /// Specifies the listener Thread-main
            /// </summary>
            public Thread ListenerThread;

            public ListenerInfo(DateTime startTime,
                Thread listenerThread)
            {
                StartTime = startTime;
                ListenerThread = listenerThread;
            }
        }

        
    }
}
