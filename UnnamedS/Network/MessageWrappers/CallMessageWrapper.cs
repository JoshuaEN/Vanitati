using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public abstract class CallMessageWrapper : MessageWrapper
    {
        public bool RequiresHost { get; }

        public CallMessageWrapper(bool requiresHost = false)
        {
            RequiresHost = requiresHost;
        }

        public abstract void Call(LocalGameLogic logic);
    }
}
