using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Client : IClient, IClientNotifier
    {
        private TcpClient TcpClient { get; }
        private NetworkStream NetworkStream { get; }
        public bool IsDisconnected { get; private set; }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<DisconnectedEventArgs> Disconnected;
        public event EventHandler<ExceptionEventArgs> Exception;

        protected void OnMessageReceived(MessageReceivedEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(null != e);

            var handler = MessageReceived;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnDisconnected(DisconnectedEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(null != e);

            var handler = Disconnected;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnException(ExceptionEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(null != e);

            var handler = Exception;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public Client(System.Net.IPEndPoint remoteEP)
        {
            Contract.Requires<ArgumentNullException>(null != remoteEP);

            TcpClient = new TcpClient();
            TcpClient.Connect(remoteEP);
            NetworkStream = TcpClient.GetStream();
        }

        public Client(TcpClient client)
        {
            TcpClient = client;
            NetworkStream = TcpClient.GetStream();
        }

        private bool _reading = false;
        public virtual async Task Listen()
        {
            if(_reading)
            {
                throw new InvalidOperationException("Already Listening");
            }
            _reading = true;

            try
            {
                await StartReading();
            }
            catch (Exception e)
            {
                Disconnect(e);
                OnException(new ExceptionEventArgs(new Exceptions.NetworkExcetpion("Error while Listening", e)));
            }
        }

        protected virtual async Task StartReading()
        {
            while (IsDisconnected == false)
            {
                byte[] headerBuffer = BitConverter.GetBytes(new Int32());
                int headerBufferRes = await Read(headerBuffer);

                if(headerBufferRes == 0)
                {
                    throw new Exceptions.ConnectionGracefullyClosedException();
                }

                if(headerBufferRes != headerBuffer.Length)
                {
                    throw new Exceptions.IncompleteHeaderException(string.Format("Expected to receive {0} bytes, got {1} bytes", headerBuffer.Length, headerBufferRes));
                }

                byte[] messageBuffer = new byte[BitConverter.ToInt32(headerBuffer, 0)];

                int messageBufferRes = await Read(messageBuffer);

                if (messageBufferRes == 0)
                {
                    throw new Exceptions.ConnectionClosedException("Connection closed while sending Message");
                }

                if(messageBufferRes != messageBuffer.Length)
                {
                    throw new Exceptions.IncompleteMessageException(string.Format("Expected to receive {0} bytes, got {1} bytes", messageBuffer.Length, messageBufferRes));
                }

#if NETWORK_LAG
                await Task.Delay(500);
#endif

                OnMessageReceived(new MessageReceivedEventArgs(Encoding.Unicode.GetString(messageBuffer)));
            }
        }

        private async Task<int> Read(byte[] buffer)
        {
            Contract.Requires<ArgumentNullException>(null != buffer);
            Contract.Requires<ArgumentException>(buffer.Length >= 1);

            var offset = 0;
            var length = buffer.Length;
            while (offset < length)
            {
                int result = await NetworkStream.ReadAsync(buffer, offset, length - offset);

                if (result == 0)
                {
                    throw new Exceptions.ConnectionClosedException("Incomplete read");
                }

                offset += result;
            }

            return offset;
        }

        public virtual void Write(string message)
        {
            try
            {
                var messageBuffer = Encoding.Unicode.GetBytes(message);
                var lengthBuffer = BitConverter.GetBytes(messageBuffer.Length);
                NetworkStream.Write(lengthBuffer, 0, lengthBuffer.Length);
                NetworkStream.Write(messageBuffer, 0, messageBuffer.Length);
            }
            catch(Exception e)
            {
                Disconnect(e);
                throw new Exceptions.NetworkExcetpion("Network Error when Sending Message", e);
            }
        }

        public virtual void Send(object obj)
        {
            Write(Serializers.Serializer.Serialize(obj));
        }

        public virtual void Disconnect()
        {
            Disconnect(new Exceptions.ConnectionGracefullyClosedException());
        }

        protected virtual void Disconnect(Exception disconnectCause)
        {
            try
            {
                Close();
            }
            catch (Exception e)
            {
                OnException(new ExceptionEventArgs(e));
            }
            finally
            {
                if (disconnectCause != null)
                    OnException(new ExceptionEventArgs(disconnectCause));

                OnDisconnected(new DisconnectedEventArgs(disconnectCause));
            }
        }

        private void Close()
        {
            try
            {
                try {
                    NetworkStream.Close();
                } catch(Exception) { }
                try {
                    TcpClient.Close();
                } catch(Exception) { }
                try {
                    TcpClient.Client.Close();
                } catch(Exception) { }
            }
            catch(Exception e)
            {
                OnException(new ExceptionEventArgs(e));
            }
            finally
            {
                IsDisconnected = true;
            }
        }

        ~Client()
        {
            Close();
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != NetworkStream);
        }
    }
}
