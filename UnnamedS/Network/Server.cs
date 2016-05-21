using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Server : IClientNotifier
    {
        private readonly QueuedLock serverLock = new QueuedLock();
        private TcpListener _listener;

        private Dictionary<ServerClient, Task> ConnectedClients = new Dictionary<ServerClient, Task>();

        private Game.LocalGameLogic Logic { get; } = new Game.LocalGameLogic();

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ExceptionEventArgs> Exception;
        public event EventHandler<DisconnectedEventArgs> Disconnected;

        private int nextUserID = 0;

        public Server(TcpListener listener)
        {
            Contract.Requires<ArgumentNullException>(null != listener);
            _listener = listener;
            _listener.Start();
        }

        private volatile bool _stop = false;

        private bool _listening = false;
        private Task _listernTask;

        public void Listen()
        {
            if (_listening)
            {
                throw new InvalidOperationException("Already Listening");
            }
            _listening = true;

            _listernTask = _listen();
        }

        private async Task _listen()
        {
            try
            {
                while (_stop == false)
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);

                    if (_stop == true)
                    {
                        break;
                    }

                    var client = new ServerClient(tcpClient, Logic, new Game.User(nextUserID++, null, nextUserID == 1), serverLock);
                    client.Disconnected += Client_Disconnected;
                    client.Exception += Client_Exception;
#if NET_DEBUG
                    client.MessageReceived += Client_MessageReceived;
#endif
                    ConnectedClients.Add(client, client.Listen());
                }
            }
            catch(Exception e)
            {
                Client_Exception(this, new ExceptionEventArgs(e));
            }
            finally
            {
                Stop();
            }
        }

        public void Stop()
        {
            if (_stop == true)
                return;

            Shutdown();
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(null != sender);
            Contract.Requires<ArgumentNullException>(null != e);

            var handler = MessageReceived;
            if(handler != null)
            {
                handler(sender, e);
            }
        }

        private void Client_Exception(object sender, ExceptionEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(null != sender);
            Contract.Requires<ArgumentNullException>(null != e);

            var handler = Exception;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private void Client_Disconnected(object sender, DisconnectedEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(null != sender);
            Contract.Requires<ArgumentNullException>(null != e);

            var handler = Disconnected;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        ~Server()
        {
            Shutdown();
        }

        private void Shutdown()
        {
            _stop = true;
            _listener.Server.Close();
            _listener.Stop();

            foreach (var client in ConnectedClients)
            {
                try
                {
                    client.Key.Disconnect();
                }
                catch (Exception) { }
            }
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != _listener);
            Contract.Invariant(null != ConnectedClients);
            Contract.Invariant(null != Logic);
        }
    }
}
