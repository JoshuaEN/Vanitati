﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network
{
    public class Client : IClientNotifier
    {
        private NetworkStream NetworkStream { get; }
        public bool IsDisconnected { get; private set; }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<DisconnectedEventArgs> Disconnected;
        public event EventHandler<ExceptionEventArgs> Exception;

        protected void OnMessageReceived(MessageReceivedEventArgs e)
        {
            var handler = MessageReceived;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnDisconnected(DisconnectedEventArgs e)
        {
            var handler = Disconnected;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnException(ExceptionEventArgs e)
        {
            var handler = Exception;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public Client(NetworkStream networkStream)
        {
            NetworkStream = networkStream;
            
        }

        public Client(System.Net.IPEndPoint remoteEP)
        {
            var tcpClient = new TcpClient();
            tcpClient.Connect(remoteEP);
            NetworkStream = tcpClient.GetStream();
        }

        private bool _reading = false;
        public async Task Listen()
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

        protected async Task StartReading()
        {
            while (true)
            {
                byte[] headerBuffer = BitConverter.GetBytes(new Int32());
                int headerBufferRes = await NetworkStream.ReadAsync(headerBuffer, 0, headerBuffer.Length);

                if(headerBufferRes == 0)
                {
                    throw new Exceptions.ConnectionGracefullyClosedException();
                }

                if(headerBufferRes != headerBuffer.Length)
                {
                    throw new Exceptions.IncompleteHeaderException(String.Format("Expected to receive {0} bytes, got {1} bytes", headerBuffer.Length, headerBufferRes));
                }

                byte[] messageBuffer = new byte[BitConverter.ToInt32(headerBuffer, 0)];
                var messageBufferOffset = 0;

                

                while (messageBufferOffset < messageBuffer.Length)
                {
                    int messageBufferRes = await NetworkStream.ReadAsync(messageBuffer, messageBufferOffset, messageBuffer.Length - messageBufferOffset);

                    if (messageBufferRes == 0)
                    {
                        throw new Exceptions.ConnectionClosedException("Connection closed while sending Message");
                    }

                    messageBufferOffset += messageBufferRes;
                }

                if(messageBufferOffset != messageBuffer.Length)
                {
                    throw new Exceptions.IncompleteMessageException(string.Format("Expected to receive {0} bytes, got {1} bytes", messageBuffer.Length, messageBufferOffset));
                }

#if NETWORK_LAG
                await Task.Delay(150);
#endif

                OnMessageReceived(new MessageReceivedEventArgs(Encoding.Unicode.GetString(messageBuffer)));
            }
        }

        //private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public void Write(string message)
        {
            Contract.Requires<ArgumentNullException>(null != message);

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

        public void Send(object obj)
        {
            Write(Serializers.Serializer.Serialize(obj));
        }

        public void Disconnect()
        {
            Disconnect(null);
        }

        protected void Disconnect(Exception disconnectCause)
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
                OnDisconnected(new DisconnectedEventArgs(disconnectCause));
            }
        }

        private void Close()
        {
            try
            {
                NetworkStream.Close();
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
    }
}
