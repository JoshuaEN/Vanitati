using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.CommanderTypes
{
    public sealed class BasicCommander : CommanderType
    {
        private BasicCommander() : base("basic") { }
        public static BasicCommander Instance { get; } = new BasicCommander();
    }
}
