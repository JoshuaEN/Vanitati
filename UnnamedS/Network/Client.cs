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
    public class Client
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
                    throw new Exceptions.IncompleteHeaderException(String.Format("Expected to receieve {0} bytes, got {1} bytes", headerBuffer.Length, headerBufferRes));
                }

                byte[] messageBuffer = new byte[BitConverter.ToInt32(headerBuffer, 0)];
                int messageBufferRes = await NetworkStream.ReadAsync(messageBuffer, 0, messageBuffer.Length);

                if(messageBufferRes == 0)
                {
                    throw new Exceptions.ConnectionClosedException("Connection closed while sending Message");
                }

                if(messageBufferRes != messageBuffer.Length)
                {
                    throw new Exceptions.IncompleteMessageException(string.Format("Expected to receieve {0} bytes, got {1} bytes", messageBuffer.Length, messageBufferRes));
                }

                OnMessageReceived(new MessageReceivedEventArgs(Encoding.Unicode.GetString(messageBuffer)));
            }
        }

        //private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public void Write(string message)
        {
            Contract.Requires<ArgumentNullException>(null != message);

            try
            {
                var buffer = Encoding.Unicode.GetBytes(message);
                NetworkStream.Write(buffer, 0, buffer.Length);
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
