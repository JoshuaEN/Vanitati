using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.SupplyTypes
{
    public sealed class Rockets : SupplyType
    {
        private Rockets() : base("rockets") { }
        public static Rockets Instance { get; } = new Rockets();
    }
}
