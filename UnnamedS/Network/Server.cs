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
        private readonly object serverLock = new object();
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

        private bool _stop = false;

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
                    var tcpClient = await _listener.AcceptSocketAsync().ConfigureAwait(false);

                    if (_stop == true)
                    {
                        break;
                    }

                    var client = new ServerClient(new NetworkStream(tcpClient), Logic, new Game.User(nextUserID++, null), serverLock);
                    client.Disconnected += Client_Disconnected;
                    client.Exception += Client_Exception;
#if DEBUG
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

            _stop = true;

            _listener.Stop();
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
            _listener.Stop();
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
