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
    public class Server
    {
        private TcpListener _listener;

        private Dictionary<ServerClient, Task> ConnectedClients = new Dictionary<ServerClient, Task>();

        private Game.LocalGameLogic Logic { get; } = new Game.LocalGameLogic(new Game.BattleGameState());

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ExceptionEventArgs> Exception;
        public event EventHandler<DisconnectedEventArgs> ClientDisconnected;

        public Server(TcpListener listener)
        {
            Contract.Requires<ArgumentNullException>(null != listener);
            _listener = listener;
        }

        private bool _listening = false;
        public async Task Listen()
        {
            if (_listening)
            {
                throw new InvalidOperationException("Already Listening");
            }

            while (true)
            {
                var tcpClient = await _listener.AcceptSocketAsync().ConfigureAwait(false);
                var client = new ServerClient(new NetworkStream(tcpClient), Logic);
                client.Disconnected += Client_Disconnected;
                client.Exception += Client_Exception;
#if DEBUG
                client.MessageReceived += Client_MessageReceived;
#endif
                ConnectedClients.Add(client, client.Listen());
            }
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var handler = MessageReceived;
            if(handler != null)
            {
                handler(sender, e);
            }
        }

        private void Client_Exception(object sender, ExceptionEventArgs e)
        {
            var handler = Exception;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private void Client_Disconnected(object sender, DisconnectedEventArgs e)
        {
            var handler = ClientDisconnected;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
