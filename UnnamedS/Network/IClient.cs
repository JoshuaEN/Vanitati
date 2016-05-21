using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network
{
    [ContractClass(typeof(ContractClassForIClient))]
    public interface IClient : IClientNotifier
    {
        bool IsDisconnected { get; }

        //event EventHandler<MessageReceivedEventArgs> MessageReceived;
        //event EventHandler<DisconnectedEventArgs> Disconnected;
        //event EventHandler<ExceptionEventArgs> Exception;

        Task Listen();
        void Write(string message);
        void Send(object obj);
        void Disconnect();
    }

    [ContractClassFor(typeof(IClient))]
    internal abstract class ContractClassForIClient : IClient
    {
        public abstract bool IsDisconnected { get; }

        public abstract event EventHandler<DisconnectedEventArgs> Disconnected;
        public abstract event EventHandler<ExceptionEventArgs> Exception;
        public abstract event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public abstract void Disconnect();

        public Task Listen()
        {
            throw new NotImplementedException();
        }

        public void Send(object obj)
        {
            Contract.Requires<ArgumentNullException>(null != obj);
            throw new NotImplementedException();
        }

        public void Write(string message)
        {
            Contract.Requires<ArgumentNullException>(null != message);
            Contract.Requires<ArgumentException>(IsDisconnected == false);
            throw new NotImplementedException();
        }
    }
}
