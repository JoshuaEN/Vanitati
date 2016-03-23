using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string MessageString { get; }

        private MessageWrappers.MessageWrapper _message = null;
        public MessageWrappers.MessageWrapper Message
        {
            get
            {
                if(_message == null)
                {
                    _message = Serializers.Serializer.Deserialize<MessageWrappers.MessageWrapper>(MessageString);
                }
                return _message;
            }
        }

        public MessageReceivedEventArgs(string message)
        {
            Contract.Requires<ArgumentNullException>(null != message);
            MessageString = message;
        }
    }
}
