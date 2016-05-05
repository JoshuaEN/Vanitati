using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    interface ITurnAwareEvent
    {
        int TurnID { get; }
    }
}
