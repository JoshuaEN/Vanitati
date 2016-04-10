using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public abstract class NotifyMessageWrapper : MessageWrapper
    {
        public abstract void Notify(IUserLogic logic);
    }
}
