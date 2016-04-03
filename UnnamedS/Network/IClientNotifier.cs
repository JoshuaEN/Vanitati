using System;

namespace UnnamedStrategyGame.Network
{
    public interface IClientNotifier
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<DisconnectedEventArgs> Disconnected;
        event EventHandler<ExceptionEventArgs> Exception;
    }
}