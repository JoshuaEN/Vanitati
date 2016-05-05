using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.SupplyTypes
{
    public sealed class Shells : SupplyType
    {
        private Shells() : base("shells") { }
        public static Shells Instance { get; } = new Shells();
    }
}
