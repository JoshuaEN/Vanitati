using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class MessageReceivedEventArgs : EventArgs
    {
        public string MessageString { get; }

        private MessageWrappers.MessageWrapper _message = null;
        public virtual MessageWrappers.MessageWrapper Message
        {
            get
            {
                Contract.Ensures(Contract.Result<MessageWrappers.MessageWrapper>() != null);
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
