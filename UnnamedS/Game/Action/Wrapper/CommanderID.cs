using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action.Wrapper
{
    public sealed class CommanderID
    {
        public int Value { get; }

        public CommanderID(int value)
        {
            Value = value;
        }
    }
}
